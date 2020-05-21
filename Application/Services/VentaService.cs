using System.Collections.Generic;
using Application.Base;
using Application.Models;
using Domain.Contracts;
using Domain.Entities;

namespace Application.Services
{
    public class VentaService : BaseService<Venta>
    {
        private ProductoService _productoService;
        public VentaService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.VentaRepository)
        {
            _productoService = new ProductoService(_unitOfWork);
        }

        public IResponse<Venta> Add(VentaRequest request)
        {
            Vendedor vendedor = _unitOfWork.VendedorRepository.FindFirstOrDefault(x => x.Persona.Documento.Numero == request.DocumentoVendedor);
            if (vendedor == null)
            {
                return new Response<Venta>
                (
                    mensaje: $"Vendedor con documento {request.DocumentoVendedor} no encontrado"
                );
            }

            List<VentaDetalle> Detalles = new List<VentaDetalle>();
            foreach (var item in request.Detalles)
            {
                if (_productoService.Disponible(item.CodigoProducto, item.CodigoBodega, item.Cantidad) == false)
                {
                    return new Response<Venta>
                    (
                        mensaje: $"El producto {item.CodigoProducto} no está disponible para esa cantidad."
                    );
                }
                VentaDetalle detalle = item.ToEntity();
                detalle.ProductoBodega = _productoService.ProductoEnBodega(item.CodigoProducto, item.CodigoBodega);
                Detalles.Add(detalle);
            }

            Venta venta = request.ToEntity();
            venta.VentaDetalles = Detalles;

            vendedor.Ventas.Add(venta);

            return new Response<Venta>
            (
                mensaje: "Venta registrada correctamente",
                entidad: venta
            );
        }
    }
}