export interface FiltroData {
    tipoFiltro: 'mes' | 'periodo';
    mesAno?: Date;
    dataInicio?: Date;
    dataFim?: Date;
  }