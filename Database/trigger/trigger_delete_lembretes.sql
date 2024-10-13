CREATE TRIGGER trg_after_delete_lembrete
AFTER DELETE ON lembretes
FOR EACH ROW
BEGIN
    INSERT INTO lembreteslog (LembreteID, Titulo, Descricao, DataLembrete, CriadoEm, IntervaloEmDias, Acao)
    VALUES (OLD.LembreteID, OLD.Titulo, OLD.Descricao, OLD.DataLembrete, OLD.CriadoEm, OLD.IntervaloEmDias, 'DELETE');
END;
