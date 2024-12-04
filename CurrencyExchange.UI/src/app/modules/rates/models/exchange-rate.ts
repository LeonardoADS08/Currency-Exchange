export interface ExchangeRate {
  lastUpdate: Date;
  base: string;
  rates: { [key: string]: number };
}
