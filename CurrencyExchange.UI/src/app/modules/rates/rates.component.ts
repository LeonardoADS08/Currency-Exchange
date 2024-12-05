import { Component, inject, DestroyRef } from '@angular/core';
import { RatesStore } from './rates.store';
import { SharedModule } from '@src/app/shared/shared.module';
import { debounceTime, combineLatest, filter, distinctUntilChanged, interval } from 'rxjs';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { CurrencyPairSelectorComponent } from './currency-pair-selector/currency-pair-selector.component';
import { RatesListComponent } from './rates-list/rates-list.component';

@Component({
  selector: 'rates',
  standalone: true,
  imports: [
    SharedModule,
    CurrencyPairSelectorComponent,
    RatesListComponent
  ],
  templateUrl: './rates.component.html',
  styleUrl: './rates.component.scss'
})
export class RatesComponent {
  private readonly destroyRef = inject(DestroyRef);
  ratesStore = inject(RatesStore);

  constructor() {
    this.ratesStore.loadCurrencies();
    this.setupRatePolling();
  }

  private setupRatePolling() {
    // Convert signals to observables
    const baseCurrency$ = toObservable(this.ratesStore.baseCurrency);
    const targetCurrencies$ = toObservable(this.ratesStore.targetCurrencies);

    // Combine and debounce currency changes
    combineLatest([baseCurrency$, targetCurrencies$]).pipe(
      debounceTime(1000),
      filter(([base, targets]) => !!base && targets.length > 0),
      distinctUntilChanged((prev, curr) =>
        prev[0] === curr[0] &&
        prev[1].length === curr[1].length &&
        prev[1].every((v, i) => v === curr[1][i])
      ),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(() => {
      this.ratesStore.loadRates();
    });

    // Set up periodic polling
    interval(60000).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(() => {
      if (this.ratesStore.hasBaseCurrency() && this.ratesStore.hasTargetCurrencies()) {
        this.ratesStore.loadRates();
      }
    });
  }
}
