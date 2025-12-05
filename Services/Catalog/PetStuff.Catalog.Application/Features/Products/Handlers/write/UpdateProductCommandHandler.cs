using AutoMapper;
using MediatR;
using PetStuff.Catalog.Application.Features.Products.Commands;
using PetStuff.Catalog.Application.Interfaces.ProductInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStuff.Catalog.Application.Features.Products.Handlers.write
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand,bool>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        public UpdateProductCommandHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.Id);
            if (product == null) return false;

            _mapper.Map(request, product);

            await _repository.UpdateProductAsync(product);

            return true;
        }
    }
}
