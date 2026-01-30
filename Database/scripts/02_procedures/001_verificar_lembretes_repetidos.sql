DELIMITER //

DROP PROCEDURE IF EXISTS `VerificarLembretesRepetidos`;

CREATE PROCEDURE `VerificarLembretesRepetidos`()
BEGIN
    DECLARE v_lembreteID INT;
    DECLARE v_usuarioID INT;
    DECLARE v_titulo VARCHAR(100);
    DECLARE v_descricao TEXT;
    DECLARE v_intervalo INT;
    DECLARE v_dataLembrete DATETIME;
    DECLARE v_criadoEm DATETIME;
    DECLARE v_dia INT;
    DECLARE v_novoLembreteID INT;
    DECLARE done INT DEFAULT 0;
    DECLARE v_erro BOOLEAN DEFAULT FALSE;

    DECLARE cur_lembretes CURSOR FOR
        SELECT LembreteID, UsuarioID, Titulo, Descricao, IntervaloEmDias, DataLembrete, CriadoEm
        FROM lembretes
        WHERE IntervaloEmDias > 0
        AND DataLembrete >= CURDATE()
        AND Ativo = 1
        ORDER BY DataLembrete ASC;

    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET v_erro = TRUE;
        INSERT INTO log_erros (procedimento, mensagem, data_erro)
        VALUES ('VerificarLembretesRepetidos', 'Erro ao processar lembrete', NOW());
        ROLLBACK;
    END;

    START TRANSACTION;
    
    OPEN cur_lembretes;

    read_loop: LOOP
        FETCH cur_lembretes INTO v_lembreteID, v_usuarioID, v_titulo, v_descricao, v_intervalo, v_dataLembrete, v_criadoEm;

        IF done OR v_erro THEN
            LEAVE read_loop;
        END IF;

        IF v_intervalo <= 0 OR v_intervalo > 365 THEN
            ITERATE read_loop;
        END IF;

        SET v_dia = 1;

        WHILE v_dia <= v_intervalo DO
            SET @nova_data = DATE_ADD(v_dataLembrete, INTERVAL v_dia DAY);
            
            IF @nova_data <= DATE_ADD(CURDATE(), INTERVAL 1 YEAR) THEN
                IF NOT EXISTS (
                    SELECT 1 
                    FROM lembretes 
                    WHERE UsuarioID = v_usuarioID 
                    AND Titulo = v_titulo 
                    AND DATE(DataLembrete) = DATE(@nova_data)
                    AND Ativo = 1
                ) THEN
                    INSERT INTO lembretes (
                        UsuarioID, 
                        Titulo, 
                        Descricao, 
                        DataLembrete, 
                        CriadoEm, 
                        IntervaloEmDias,
                        Ativo,
                        LembretePaiID
                    )
                    VALUES (
                        v_usuarioID, 
                        v_titulo, 
                        v_descricao, 
                        @nova_data, 
                        NOW(), 
                        0,
                        1,
                        v_lembreteID
                    );

                    SET v_novoLembreteID = LAST_INSERT_ID();

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

            SET v_dia = v_dia + 1;
        END WHILE;
        
        UPDATE lembretes 
        SET UltimoProcessamento = NOW()
        WHERE LembreteID = v_lembreteID;

    END LOOP;

    CLOSE cur_lembretes;

    IF NOT v_erro THEN
        COMMIT;
    END IF;

END //

DELIMITER ;
