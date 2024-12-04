import { Currency } from "./models/currency";
import { patchState, signalStore, withComputed, withHooks, withMethods, withState } from "@ngrx/signals";
import { RatesService } from "./services/rates.service";
import { computed, inject } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { catchError, debounceTime, delay, distinctUntilChanged, take, takeLast } from "rxjs";
interface RatesState {
  baseCurrency: string | null;
  targetCurrencies: string[];
  currencies: Currency[];
  exchangeRates: { [key: string]: number };
  updateInterval: number;
  loading: boolean;
  error: boolean;
}

const ratesStore: RatesState = {
  baseCurrency: null,
  targetCurrencies: [],
  currencies: [],
  exchangeRates: {},
  updateInterval: 10000,
  loading: false,
  error: false,
}


export const RatesStore = signalStore(
  { providedIn: 'root' },
  withState(ratesStore),
  withComputed((store) => ({
    hasBaseCurrency: computed(() => !!store.baseCurrency()),
    hasTargetCurrencies: computed(() => store.targetCurrencies().length > 0)
  })),
  withMethods((store, ratesService = inject(RatesService)) => ({
    setBaseCurrency(baseCurrency: string) {
      patchState(store, { baseCurrency: baseCurrency });
    },
    setTargetCurrencies(targetCurrencies: string[]) {
      patchState(store, { targetCurrencies: targetCurrencies });
    },
    setUpdateInterval(updateInterval: number) {
      patchState(store, { updateInterval: updateInterval });
    },
    loadRates() {
      patchState(store, { loading: true });

      ratesService.getExchangeRates(
        store.baseCurrency()!,
        store.targetCurrencies()!
      ).pipe(
        delay(1000),
        debounceTime(1000),
        distinctUntilChanged(),
        catchError(error => {
          patchState(store, { loading: false, error: true });
          throw error;
        })
      ).subscribe(rates => {
        patchState(store, { exchangeRates: rates.rates, loading: false, error: false });
      });
    },

    loadCurrencies() {
      ratesService.getCurrencies()
        .pipe(take(1))
        .subscribe(currencies => {
          patchState(store, { currencies: currencies });
        });
    }
  })),
  // withHooks({
  //   onInit: (store) => {
  //     const token = localStorage.getItem('token');
  //     const user = JSON.parse(localStorage.getItem('user')!);

  //     if (token && user) {
  //       store.setUser(user, token);
  //     }
  //   }
  // })
)
