-- Trigger para UPDATE
DELIMITER //

CREATE TRIGGER trg_after_update_lembrete
AFTER UPDATE ON lembretes
FOR EACH ROW
BEGIN
    INSERT INTO lembreteslog (
        LembreteID, 
        Titulo, 
        Descricao, 
        DataLembrete, 
        CriadoEm, 
        IntervaloEmDias, 
        Acao,
        DataAcao,
        UsuarioID,
        ValoresAntigos
    )
    VALUES (
        NEW.LembreteID, 
        NEW.Titulo, 
        NEW.Descricao, 
        NEW.DataLembrete, 
        NEW.CriadoEm, 
        NEW.IntervaloEmDias, 
        'UPDATE',
        NOW(),
        NEW.UsuarioID,
        CONCAT(
            'Título anterior: ', OLD.Titulo, ', ',
            'Descrição anterior: ', OLD.Descricao, ', ',
            'Data anterior: ', OLD.DataLembrete, ', ',
            'Intervalo anterior: ', OLD.IntervaloEmDias
        )
    );
END //

DELIMITER ;