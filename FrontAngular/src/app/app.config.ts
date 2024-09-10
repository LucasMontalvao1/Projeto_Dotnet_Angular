import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideClientHydration } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';  // Correção aqui
import { routes } from './app-routing.module';  // Certifique-se de que as rotas estão corretamente importadas

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),  // Passando as rotas corretamente
    provideClientHydration(),  // Hidratação para SSR (renderização no servidor)
    provideAnimations()  // Carregamento correto das animações
  ]
};
