const apiUrl = 'https://localhost:7103';

export const environment = {
  production: false,
  apiUrl: apiUrl,
  endpoints: {
    login: `${apiUrl}/Api/v1/login`
  },
};