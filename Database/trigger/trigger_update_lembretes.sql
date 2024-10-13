CREATE TRIGGER trg_after_update_lembrete
AFTER UPDATE ON lembretes
FOR EACH ROW
BEGIN
    INSERT INTO lembreteslog (LembreteID, Titulo, Descricao, DataLembrete, CriadoEm, IntervaloEmDias, Acao)
    VALUES (NEW.LembreteID, NEW.Titulo, NEW.Descricao, NEW.DataLembrete, NEW.CriadoEm, NEW.IntervaloEmDias, 'UPDATE');
END;
