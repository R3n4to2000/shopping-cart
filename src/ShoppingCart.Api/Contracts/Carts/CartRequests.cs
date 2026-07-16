using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Api.Contracts.Carts;

public sealed record AddCartItemRequest(
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "O identificador do produto deve ser maior que zero.")]
    int ProductId,

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "A quantidade deve ser maior que zero.")]
    int Quantity);

public sealed record ChangeCartItemQuantityRequest(
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "A quantidade deve ser maior que zero.")]
    int Quantity);

public sealed record ApplyCouponRequest(
    [Required(
        AllowEmptyStrings = false,
        ErrorMessage = "O código do cupom é obrigatório.")]
    [StringLength(
        30,
        ErrorMessage = "O código do cupom deve possuir no máximo 30 caracteres.")]
    string Code);