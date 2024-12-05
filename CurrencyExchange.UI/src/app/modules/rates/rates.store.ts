import { Currency } from "./models/currency";
import { patchState, signalStore, withComputed, withHooks, withMethods, withState } from "@ngrx/signals";
import { RatesService } from "./services/rates.service";
import { computed, inject } from "@angular/core";
import { catchError,take } from "rxjs";
interface RatesState {
  baseCurrency: string | null;
  targetCurrencies: string[];
  currencies: Currency[];
  exchangeRates: { [key: string]: number } | null;
  previousRates: { [key: string]: number } | null;
  updateInterval: number;
  loading: boolean;
  error: boolean;
  lastUpdate: Date | null;
}

const ratesStore: RatesState = {
  baseCurrency: null,
  targetCurrencies: [],
  currencies: [],
  exchangeRates: null,
  previousRates: null,
  updateInterval: 10000,
  loading: false,
  error: false,
  lastUpdate: null
}


export const RatesStore = signalStore(
  { providedIn: 'root' },
  withState(ratesStore),
  withComputed((store) => ({
    hasBaseCurrency: computed(() => !!store.baseCurrency()),
    hasTargetCurrencies: computed(() => store.targetCurrencies().length > 0),
    hasCurrencies: computed(() => store.currencies().length > 0),
    hasExchangeRates: computed(() => !!store.exchangeRates()),
    hasLastUpdate: computed(() => !!store.lastUpdate()),
    currencyDictionary: computed<{ [key: string]: Currency }>(() => {
      const dictionary: { [key: string]: Currency } = {};
      store.currencies().forEach(currency => {
        dictionary[currency.code] = currency;
      });
      return dictionary;
    }),
  })),
  withMethods((store, ratesService = inject(RatesService)) => ({
    setBaseCurrency(baseCurrency: string) {
      patchState(store, { baseCurrency: baseCurrency });
    },
    setTargetCurrencies(targetCurrencies: string[]) {
      patchState(store, { targetCurrencies: targetCurrencies, previousRates: {} });
    },
    setUpdateInterval(updateInterval: number) {
      patchState(store, { updateInterval: updateInterval });
    },
    loadRates() {
      if (!store.baseCurrency() || !store.targetCurrencies().length) {
        return;
      }

      patchState(store, { loading: true, error: false });

      ratesService.getExchangeRates(store.baseCurrency()!, store.targetCurrencies()).pipe(
        take(1),
        catchError(error => {
          patchState(store, {
            loading: false,
            error: true,
            lastUpdate: new Date(),
            exchangeRates: {},
            previousRates: {}
          });
          throw error;
        })
      ).subscribe(response => {
        const previousRates = store.exchangeRates();

        patchState(store, {
          exchangeRates: response.rates,
          previousRates: previousRates,
          loading: false,
          error: false,
          lastUpdate: new Date()
        });
      });
    },

    loadCurrencies() {
      patchState(store, { loading: true, error: false });

      ratesService.getCurrencies()
        .pipe(take(1))
        .subscribe(currencies => {
          patchState(store, { loading: false, error: false, currencies: currencies });
        });
    }
  })),
);
