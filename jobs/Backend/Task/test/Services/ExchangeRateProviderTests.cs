﻿using ExchangeRateUpdater.Models;
using ExchangeRateUpdater.Services;
using Moq;
using Xunit;

namespace ExchangeRateUpdaterTest.Services;

public class ExchangeRateUpdaterTests
{
    private readonly Mock<ICnbApiService> _cnbApiServiceMock;
    private readonly ExchangeRateProvider _exchangeRateProvider;

    public ExchangeRateUpdaterTests()
    {
        _cnbApiServiceMock = new Mock<ICnbApiService>();
        _exchangeRateProvider = new ExchangeRateProvider(_cnbApiServiceMock.Object);
    }


    [Fact]
    public async Task GetExchangeRates_ShouldReturnExchangeRates_WhenCurrenciesAreProvided()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var cnbRates = new CnbRateDailyResponse
        {
            Rates = new List<CnbRate>
            {
                new CnbRate { CurrencyCode = "USD", Rate = 22.5m, Amount = 1 },
                new CnbRate { CurrencyCode = "EUR", Rate = 25.5m, Amount = 1 }
            }
        };

        var currencies = new List<Currency>
        {
            new Currency("USD"),
            new Currency("EUR")
        };

        _cnbApiServiceMock
            .Setup(service => service.GetExchangeRate(cancellationToken))
            .ReturnsAsync(cnbRates);

        // Act
        var result = await _exchangeRateProvider.GetExchangeRates(currencies, cancellationToken);

        // Assert
        var exchangeRates = result.ToList();
        Assert.Equal(2, exchangeRates.Count);
        Assert.Contains(exchangeRates, rate => rate.TargetCurrency.Code == "USD" && rate.Value == 22.5m);
        Assert.Contains(exchangeRates, rate => rate.TargetCurrency.Code == "EUR" && rate.Value == 25.5m);
    }


    [Fact]
    public async Task GetExchangeRates_ShouldIgnoreUndefinedCurrencies()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var cnbRates = new CnbRateDailyResponse
        {
            Rates = new List<CnbRate>
            {
                new CnbRate { CurrencyCode = "USD", Rate = 22.5m, Amount = 1 }
            }
        };

        var currencies = new List<Currency>
        {
            new Currency("USD"),
            new Currency("EUR")
        };

        _cnbApiServiceMock
            .Setup(service => service.GetExchangeRate(cancellationToken))
            .ReturnsAsync(cnbRates);

        // Act
        var result = await _exchangeRateProvider.GetExchangeRates(currencies, cancellationToken);

        // Assert
        var exchangeRates = result.ToList();
        Assert.Single(exchangeRates);
        Assert.Contains(exchangeRates, rate => rate.TargetCurrency.Code == "USD" && rate.Value == 22.5m);
        Assert.DoesNotContain(exchangeRates, rate => rate.TargetCurrency.Code == "EUR");
    }
}