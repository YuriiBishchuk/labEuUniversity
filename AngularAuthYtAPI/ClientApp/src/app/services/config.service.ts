import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  apiUrl = 'https://localhost:7058'; // Замініть це на URL вашого API

  constructor() {}
}
