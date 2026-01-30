DELIMITER //

DROP TRIGGER IF EXISTS trg_after_insert_lembrete;

CREATE TRIGGER trg_after_insert_lembrete
AFTER INSERT ON lembretes
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
        UsuarioID
    )
    VALUES (
        NEW.LembreteID, 
        NEW.Titulo, 
        NEW.Descricao, 
        NEW.DataLembrete, 
        NEW.CriadoEm, 
        NEW.IntervaloEmDias, 
        'INSERT',
        NOW(),
        NEW.UsuarioID
    );
END //

DELIMITER ;
