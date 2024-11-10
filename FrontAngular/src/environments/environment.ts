const apiUrl = 'https://localhost:7103/api/v1';

export const environment = {
  production: false,
  apiUrl: apiUrl,
  endpoints: {
    login: `${apiUrl}/login`,
    lembretes: `${apiUrl}/lembretes`,
    transacao: `${apiUrl}/transacao`,
    categoria: `${apiUrl}/categoria`,
  },
};