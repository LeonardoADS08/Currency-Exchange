import { Component, computed, inject } from '@angular/core';
import { SharedModule } from '@src/app/shared/shared.module';
import { TableModule } from 'primeng/table';
import { RatesStore } from '../rates.store';

interface RateInfo {
  code: string;
  pair: string;
  value: number;
  changePercentage: number | null;
  change: number | null;
}
@Component({
  selector: 'rates-list',
  standalone: true,
  imports: [
    SharedModule,
    TableModule,
  ],
  templateUrl: './rates-list.component.html',
  styleUrl: './rates-list.component.scss',
})
export class RatesListComponent {
  ratesStore = inject(RatesStore);

  rates = computed<RateInfo[]>(() => {
    const currentRates = this.ratesStore.exchangeRates();
    const prevRates = this.ratesStore.previousRates();
    const dictionary = this.ratesStore.currencyDictionary();
    const baseCurrency = this.ratesStore.baseCurrency();

    if (!currentRates || !dictionary) {
      return [];
    }

    const result = Object.entries(currentRates).map(([currencyCode, value]) => {
      const currency = dictionary[currencyCode];
      if (!currency) return null;

      const previousValue = prevRates?.[currencyCode];
      let changePercentage: number | null = null;
      let change: number | null = null;

      if (previousValue !== undefined && previousValue !== 0) {
        changePercentage = Number(((value - previousValue) / previousValue * 100).toFixed(8));
        change = value - previousValue;
      }

      return {
        code: currencyCode,
        pair: `${baseCurrency}/${currencyCode}`,
        value: value,
        changePercentage: changePercentage,
        change: change
      };
    }) as RateInfo[];

    console.log('result', result);
    return result
  });

}
