using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStuff.Catalog.Application.Features.Products.Commands
{
    public class RemoveProductCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public RemoveProductCommand(int id)
        {
            Id = id;
            }
    }
}
