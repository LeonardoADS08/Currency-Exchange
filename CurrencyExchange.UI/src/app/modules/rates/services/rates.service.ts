import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '@src/environments/environment';
import { Observable } from 'rxjs';
import { ExchangeRate } from '../models/exchange-rate';
import { Currency } from '../models/currency';

@Injectable({
  providedIn: 'root'
})
export class RatesService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/CurrencyExchange`;

  getExchangeRates(baseCurrency: string, targetCurrencies: string[]): Observable<ExchangeRate> {
    const params = new HttpParams()
      .set('baseCurrency', baseCurrency)
      .set('targetCurrencies', targetCurrencies.join(','));

    return this.http.get<ExchangeRate>(`${this.apiUrl}/rates`, { params });
  }

  getCurrencies(): Observable<Currency[]> {
    return this.http.get<Currency[]>(`${this.apiUrl}/currencies`);
  }
}
