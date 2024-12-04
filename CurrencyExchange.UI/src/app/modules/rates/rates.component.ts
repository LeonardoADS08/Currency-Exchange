import { Component, computed, effect, inject, signal, untracked } from '@angular/core';
import { RatesStore } from './rates.store';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { TableModule } from 'primeng/table';
import { Currency } from './models/currency';
import { SharedModule } from '@src/app/shared/shared.module';

interface RateInfo {
  code: string;
  pair: string;
  value: number;
  changePercentage: number | null;
}

@Component({
  selector: 'rates',
  standalone: true,
  imports: [
    SharedModule,
    DropdownModule,
    MultiSelectModule,
    TableModule,
  ],
  templateUrl: './rates.component.html',
  styleUrl: './rates.component.scss'
})
export class RatesComponent {

  selectedBase = signal<Currency | null>(null);
  selectedTargets = signal<Currency[]>([]);
  previousRates = signal< { [key: string]: number } | null>(null);

  ratesStore = inject(RatesStore);

  rates = computed<RateInfo[]>(() => {
    const currentRates = this.ratesStore.exchangeRates();
    const prevRates = this.previousRates();

    const rates = Object.entries(currentRates).map(([currency, value]) => {
      const previousValue = prevRates?.[currency];
      let changePercentage: number | null = null;

      if (previousValue !== undefined && previousValue !== 0) {
        changePercentage = ((value - previousValue) / previousValue) * 100;
      }

      return {
        code: currency,
        pair: `${this.ratesStore.baseCurrency()}/${currency}`,
        value: value,
        changePercentage: changePercentage
      } as RateInfo;
    });

    // Store current rates as previous for next update
    untracked(() =>this.previousRates.set(currentRates));

    return rates;
  });

  constructor() {
    this.ratesStore.loadCurrencies();

    effect(() => {
      if (this.ratesStore.hasBaseCurrency() && this.ratesStore.hasTargetCurrencies()) {
        this.ratesStore.loadRates();
      }
    }, { allowSignalWrites: true });
  }

  updateTargetCurrencies($event: Currency[]) {
    this.ratesStore.setTargetCurrencies($event.map(currency => currency.code));
  }
}
