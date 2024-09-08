import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';  // Importa o HttpClientModule
import { AppComponent } from './app.component';

@NgModule({
  declarations: [
    AppComponent,  // Adicione seus componentes aqui
    // Outros componentes
  ],
  imports: [
    BrowserModule,
    HttpClientModule  // Adicione o HttpClientModule aqui
  ],
  providers: [],
  bootstrap: [AppComponent]  // Componente inicial da aplicação
})
export class AppModule { }
