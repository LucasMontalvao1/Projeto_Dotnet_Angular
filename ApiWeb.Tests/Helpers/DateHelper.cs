using System;

namespace ApiWeb.Helpers
{
    /// <summary>
    /// Classe utilitária para manipulação de datas em transações financeiras
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        /// Calcula a diferença em dias entre duas datas
        /// </summary>
        /// <param name="startDate">Data inicial</param>
        /// <param name="endDate">Data final</param>
        /// <returns>Número de dias entre as datas</returns>
        /// <exception cref="ArgumentException">Lançada quando a data final é anterior à data inicial</exception>
        public static int CalculateDateDifference(DateTime startDate, DateTime endDate)
        {
            if (endDate.Date < startDate.Date)
            {
                throw new ArgumentException("A data final não pode ser anterior à data inicial");
            }

            return (endDate.Date - startDate.Date).Days;
        }

        /// <summary>
        /// Verifica se a data da transação é válida (não futura)
        /// </summary>
        /// <param name="date">Data a ser validada</param>
        /// <returns>True se a data for válida, False caso contrário</returns>
        public static bool IsValidTransactionDate(DateTime date)
        {
            return date.Date <= DateTime.Now.Date;
        }

        /// <summary>
        /// Obtém o primeiro dia do mês para uma data específica
        /// </summary>
        /// <param name="date">Data de referência</param>
        /// <returns>Primeiro dia do mês</returns>
        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Obtém o último dia do mês para uma data específica
        /// </summary>
        /// <param name="date">Data de referência</param>
        /// <returns>Último dia do mês</returns>
        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Verifica se uma data está dentro de um intervalo específico
        /// </summary>
        /// <param name="date">Data a ser verificada</param>
        /// <param name="startDate">Data inicial do intervalo</param>
        /// <param name="endDate">Data final do intervalo</param>
        /// <returns>True se a data estiver no intervalo, False caso contrário</returns>
        public static bool IsDateInRange(DateTime date, DateTime startDate, DateTime endDate)
        {
            return date.Date >= startDate.Date && date.Date <= endDate.Date;
        }

        /// <summary>
        /// Obtém o primeiro dia do mês atual
        /// </summary>
        /// <returns>Primeiro dia do mês atual</returns>
        public static DateTime GetFirstDayOfCurrentMonth()
        {
            return GetFirstDayOfMonth(DateTime.Now);
        }

        /// <summary>
        /// Obtém o último dia do mês atual
        /// </summary>
        /// <returns>Último dia do mês atual</returns>
        public static DateTime GetLastDayOfCurrentMonth()
        {
            return GetLastDayOfMonth(DateTime.Now);
        }

        /// <summary>
        /// Verifica se a data está dentro do mês atual
        /// </summary>
        /// <param name="date">Data a ser verificada</param>
        /// <returns>True se a data estiver no mês atual, False caso contrário</returns>
        public static bool IsCurrentMonth(DateTime date)
        {
            var now = DateTime.Now;
            return date.Year == now.Year && date.Month == now.Month;
        }

        /// <summary>
        /// Retorna o número de meses entre duas datas
        /// </summary>
        /// <param name="startDate">Data inicial</param>
        /// <param name="endDate">Data final</param>
        /// <returns>Número de meses entre as datas</returns>
        /// <exception cref="ArgumentException">Lançada quando a data final é anterior à data inicial</exception>
        public static int GetMonthsBetween(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                throw new ArgumentException("A data final não pode ser anterior à data inicial");
            }

            return ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
        }
    }
}