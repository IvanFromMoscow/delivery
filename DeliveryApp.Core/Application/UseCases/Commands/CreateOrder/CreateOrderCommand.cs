using CSharpFunctionalExtensions;
using MediatR;
using Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Result<bool, Error>>
    {
        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="basketId">Идентификатор корзины</param>
        /// <param name="street">Улица</param>
        /// <exception cref="ArgumentException"></exception>
        public CreateOrderCommand(Guid basketId, string street)
        {
            if (basketId == Guid.Empty) throw new ArgumentException(nameof(basketId));
            if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException(nameof(street));

            BasketId = basketId;
            Street = street;
        }

        /// <summary>
        /// Идентификатор корзины
        /// </summary>
        /// <remarks>Id корзины берется за основу при создании Id заказа, они совпадают</remarks>
        public Guid BasketId { get; private set; }

        /// <summary>
        /// Улица
        /// </summary>
        public string Street { get; private set; }


    }
}
