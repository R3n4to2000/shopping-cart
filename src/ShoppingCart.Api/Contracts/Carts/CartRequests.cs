using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Api.Contracts.Carts;

public sealed record AddCartItemRequest(
    [property: Range(
        1,
        int.MaxValue,
        ErrorMessage = "O identificador do produto deve ser maior que zero.")]
    int ProductId,

    [property: Range(
        1,
        int.MaxValue,
        ErrorMessage = "A quantidade deve ser maior que zero.")]
    int Quantity);

public sealed record ChangeCartItemQuantityRequest(
    [property: Range(
        1,
        int.MaxValue,
        ErrorMessage = "A quantidade deve ser maior que zero.")]
    int Quantity);

public sealed record ApplyCouponRequest(
    [property: Required(
        AllowEmptyStrings = false,
        ErrorMessage = "O código do cupom é obrigatório.")]
    [property: StringLength(
        30,
        ErrorMessage = "O código do cupom deve possuir no máximo 30 caracteres.")]
    string Code);