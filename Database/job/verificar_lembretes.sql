DELIMITER //

CREATE DEFINER=`root`@`localhost` PROCEDURE `VerificarLembretesRepetidos`()
BEGIN
    -- Declaração de variáveis que serão usadas para armazenar os dados do lembrete atual
    DECLARE v_lembreteID INT;          -- ID do lembrete sendo processado
    DECLARE v_usuarioID INT;           -- ID do usuário dono do lembrete
    DECLARE v_titulo VARCHAR(100);      -- Título do lembrete
    DECLARE v_descricao TEXT;          -- Descrição do lembrete
    DECLARE v_intervalo INT;           -- Intervalo em dias para repetição
    DECLARE v_dataLembrete DATETIME;   -- Data do lembrete original
    DECLARE v_criadoEm DATETIME;       -- Data de criação do lembrete
    DECLARE v_dia INT;                 -- Contador para o loop de dias
    DECLARE v_novoLembreteID INT;      -- ID do novo lembrete criado
    DECLARE done INT DEFAULT 0;        -- Flag para controle do cursor
    DECLARE v_erro BOOLEAN DEFAULT FALSE; -- Flag para controle de erros

    -- Declaração do cursor que vai buscar os lembretes que precisam ser repetidos
    -- Seleciona apenas lembretes ativos, com intervalo > 0 e data futura ou atual
    DECLARE cur_lembretes CURSOR FOR
        SELECT LembreteID, UsuarioID, Titulo, Descricao, IntervaloEmDias, DataLembrete, CriadoEm
        FROM lembretes
        WHERE IntervaloEmDias > 0          -- Apenas lembretes com repetição
        AND DataLembrete >= CURDATE()      -- Apenas lembretes futuros ou do dia atual
        AND Ativo = 1                      -- Apenas lembretes ativos
        ORDER BY DataLembrete ASC;         -- Processa primeiro os mais próximos

    -- Handler para quando não houver mais registros para processar
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

    -- Handler para tratamento de erros SQL
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET v_erro = TRUE;
        -- Registra o erro em uma tabela de log
        INSERT INTO log_erros (procedimento, mensagem, data_erro)
        VALUES ('VerificarLembretesRepetidos', 'Erro ao processar lembrete', NOW());
        -- Desfaz todas as alterações em caso de erro
        ROLLBACK;
    END;

    -- Inicia uma transação para garantir a consistência dos dados
    START TRANSACTION;
    
    -- Abre o cursor para começar a processar os lembretes
    OPEN cur_lembretes;

    -- Loop principal que vai processar cada lembrete encontrado
    read_loop: LOOP
        -- Busca o próximo lembrete
        FETCH cur_lembretes INTO v_lembreteID, v_usuarioID, v_titulo, v_descricao, v_intervalo, v_dataLembrete, v_criadoEm;

        -- Se não houver mais lembretes ou ocorreu erro, sai do loop
        IF done OR v_erro THEN
            LEAVE read_loop;
        END IF;

        -- Valida se o intervalo está dentro dos limites aceitáveis (1 a 365 dias)
        IF v_intervalo <= 0 OR v_intervalo > 365 THEN
            ITERATE read_loop;  -- Pula para o próximo lembrete se intervalo inválido
        END IF;

        -- Inicializa o contador de dias
        SET v_dia = 1;

        -- Loop que vai criar os lembretes para cada dia do intervalo
        WHILE v_dia <= v_intervalo DO
            -- Calcula a nova data somando os dias ao lembrete original
            SET @nova_data = DATE_ADD(v_dataLembrete, INTERVAL v_dia DAY);
            
            -- Verifica se a nova data não ultrapassa 1 ano
            IF @nova_data <= DATE_ADD(CURDATE(), INTERVAL 1 YEAR) THEN
                -- Verifica se já existe um lembrete igual nesta data
                IF NOT EXISTS (
                    SELECT 1 
                    FROM lembretes 
                    WHERE UsuarioID = v_usuarioID 
                    AND Titulo = v_titulo 
                    AND DATE(DataLembrete) = DATE(@nova_data)
                    AND Ativo = 1
                ) THEN
                    -- Insere o novo lembrete
                    INSERT INTO lembretes (
                        UsuarioID, 
                        Titulo, 
                        Descricao, 
                        DataLembrete, 
                        CriadoEm, 
                        IntervaloEmDias,
                        Ativo,
                        LembretePaiID      -- Referência ao lembrete original
                    )
                    VALUES (
                        v_usuarioID, 
                        v_titulo, 
                        v_descricao, 
                        @nova_data, 
                        NOW(), 
                        0,                  -- Novos lembretes não têm intervalo
                        1,                  -- Criado como ativo
                        v_lembreteID        -- ID do lembrete pai
                    );

                    -- Guarda o ID do novo lembrete criado
                    SET v_novoLembreteID = LAST_INSERT_ID();

                    -- Registra a execução para fins de auditoria
                    INSERT INTO lembretesexecucao (
                        LembreteID, 
                        NovoLembreteID, 
                        DataExecucao,
                        Status
                    )
                    VALUES (
                        v_lembreteID, 
                        v_novoLembreteID, 
                        NOW(),
                        'CRIADO'
                    );
                END IF;
            END IF;

            -- Incrementa o contador de dias
            SET v_dia = v_dia + 1;
        END WHILE;
        
        -- Atualiza a data do último processamento do lembrete original
        UPDATE lembretes 
        SET UltimoProcessamento = NOW()
        WHERE LembreteID = v_lembreteID;

    END LOOP;

    -- Fecha o cursor
    CLOSE cur_lembretes;

    -- Se não houve erros, confirma todas as alterações
    IF NOT v_erro THEN
        COMMIT;
    END IF;

END //

DELIMITER ;