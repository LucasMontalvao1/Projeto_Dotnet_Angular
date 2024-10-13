CREATE DEFINER=`root`@`localhost` PROCEDURE `VerificarLembretesRepetidos`()
BEGIN
    DECLARE v_lembreteID INT;
    DECLARE v_usuarioID INT;
    DECLARE v_titulo VARCHAR(100);
    DECLARE v_descricao TEXT;
    DECLARE v_intervalo INT;
    DECLARE v_dataLembrete DATETIME;
    DECLARE v_criadoEm DATETIME;
    DECLARE v_dia INT; -- Variável do loop
    DECLARE v_novoLembreteID INT; -- Armazena o ID do novo lembrete criado
    DECLARE done INT DEFAULT 0;

    -- Cursor para selecionar lembretes com intervalo
    DECLARE cur_lembretes CURSOR FOR
        SELECT LembreteID, UsuarioID, Titulo, Descricao, IntervaloEmDias, DataLembrete, CriadoEm
        FROM lembretes
        WHERE IntervaloEmDias > 0;

    -- Handler para capturar quando não há mais linhas no cursor
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

    -- Abre o cursor
    OPEN cur_lembretes;

    -- Itera sobre cada lembrete
    read_loop: LOOP
        FETCH cur_lembres INTO v_lembreteID, v_usuarioID, v_titulo, v_descricao, v_intervalo, v_dataLembrete, v_criadoEm;

        -- Se não houver mais resultados, sai do loop
        IF done THEN
            LEAVE read_loop;
        END IF;

        -- Inicia o contador de dias
        SET v_dia = 1;

        -- Gera lembretes para os dias dentro do intervalo
        WHILE v_dia <= v_intervalo DO
            -- Calcula a nova data para o lembrete
            SET @nova_data = DATE_ADD(v_dataLembrete, INTERVAL v_dia DAY);

            -- Verifica se já existe um lembrete gerado para esse dia
            IF NOT EXISTS (
                SELECT 1 FROM lembretes 
                WHERE UsuarioID = v_usuarioID 
                AND Titulo = v_titulo 
                AND DataLembrete = @nova_data
            ) THEN
                -- Cria um novo lembrete com a nova data
                INSERT INTO lembretes (UsuarioID, Titulo, Descricao, DataLembrete, CriadoEm, IntervaloEmDias)
                VALUES (v_usuarioID, v_titulo, v_descricao, @nova_data, NOW(), 0);

                -- Obtém o ID do lembrete criado
                SET v_novoLembreteID = LAST_INSERT_ID();

                -- Registra a execução na tabela de execuções
                INSERT INTO lembretesexecucao (LembreteID, NovoLembreteID, DataExecucao)
                VALUES (v_lembreteID, v_novoLembreteID, NOW());

                -- Simula uma ação, como enviar um lembrete
                SELECT CONCAT('Novo lembrete gerado a partir do Lembrete ', v_lembreteID, ' para a data ', @nova_data, ' com ID ', v_novoLembreteID);
            END IF;

            -- Incrementa o contador de dias
            SET v_dia = v_dia + 1;
        END WHILE;
    END LOOP;

    -- Fecha o cursor
    CLOSE cur_lembretes;
END;
