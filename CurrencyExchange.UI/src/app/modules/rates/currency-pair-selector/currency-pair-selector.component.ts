import { Component, computed, inject } from '@angular/core';
import { SharedModule } from '@src/app/shared/shared.module';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { RatesStore } from '../rates.store';
import { Currency } from '../models/currency';

@Component({
  selector: 'currency-pair-selector',
  standalone: true,
  imports: [
    SharedModule,
    DropdownModule,
    MultiSelectModule,
  ],
  templateUrl: './currency-pair-selector.component.html',
  styleUrl: './currency-pair-selector.component.scss'
})
export class CurrencyPairSelectorComponent {
  ratesStore = inject(RatesStore);
  selectedBase = computed(() => this.ratesStore.currencies().find(c => c.code === this.ratesStore.baseCurrency()));
  selectedTargets = computed(() => {
    const targetCodes = this.ratesStore.targetCurrencies();
    return this.ratesStore.currencies().filter(c => targetCodes.includes(c.code));
  });

  updateTargetCurrencies($event: Currency[]) {
    this.ratesStore.setTargetCurrencies($event.map(currency => currency.code));
  }
}
