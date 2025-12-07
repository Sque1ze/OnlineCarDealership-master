using Domain.Customers;

namespace Tests.Data.Customers;

public static class CustomersData
{
    public static Customer FirstTestCustomer()
        => Customer.New(
            CustomerId.New(),
            "John",
            "Doe",
            "john.doe@example.com",
            "+380501234567",
            "Kyiv, Ukraine");

    public static Customer SecondTestCustomer()
        => Customer.New(
            CustomerId.New(),
            "Jane",
            "Smith",
            "jane.smith@example.com",
            "+380671234567",
            "Lviv, Ukraine");

    public static Customer ThirdTestCustomer()
        => Customer.New(
            CustomerId.New(),
            "Bob",
            "Johnson",
            "bob.johnson@example.com",
            "+380931234567",
            "Odesa, Ukraine");
}
