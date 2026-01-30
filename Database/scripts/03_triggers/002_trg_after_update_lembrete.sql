DELIMITER //

DROP TRIGGER IF EXISTS trg_after_update_lembrete;

CREATE TRIGGER trg_after_update_lembrete
AFTER UPDATE ON lembretes
FOR EACH ROW
BEGIN
    INSERT INTO lembreteslog (
        LembreteID, 
        UsuarioID,
        Titulo, 
        Descricao, 
        DataLembrete, 
        CriadoEm, 
        IntervaloEmDias, 
        Acao,
        DataAcao
    )
    VALUES (
        NEW.LembreteID, 
        NEW.UsuarioID,
        NEW.Titulo, 
        NEW.Descricao, 
        NEW.DataLembrete, 
        NEW.CriadoEm, 
        NEW.IntervaloEmDias, 
        'UPDATE',
        NOW()
    );
END //

DELIMITER ;
