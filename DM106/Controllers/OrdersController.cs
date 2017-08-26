using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DM106.Models;
using DM106.br.com.correios.ws;
using DM106.CRMClient;
using System.Globalization;

namespace DM106.Controllers
{
    [Authorize]
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public List<Order> GetOrders()
        {
            return db.Orders.Include(order => order.OrderItems).ToList();
        }
        
        // GET: api/Orders/5
        [Authorize]
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            if (User.IsInRole("USER"))
            {
                String user = User.Identity.Name;
                Order order = db.Orders.Where(e => e.userEmail == user && e.Id == id).FirstOrDefault();

                if (order == null)
                {
                    return Ok("Pedido não Encontrado");
                }
                return Ok(order);
            }
            Order orderAdmin = db.Orders.Find(id);

            if (orderAdmin == null)
            {
                return Ok("Pedido não Encontrado");
            }

            return Ok(orderAdmin);
        }

        //GET: api/Orders/email
        [Authorize]
        [ResponseType(typeof(Order))]
        [HttpGet]
        [Route("byemail")]
        public IHttpActionResult GetOrdersByEmail(string email)
        {
            if ((User.IsInRole("ADMIN")) || (email == User.Identity.Name))
            {
                Order order = db.Orders.Where(p => p.userEmail == email).FirstOrDefault();
                if (order == null)
                {
                    return Ok("Não há pedidos relacionados ao e-mail");
                }
                return Ok(db.Orders.Where(p => p.userEmail == email).ToList());
            }
            return Ok("Não autorizado.");
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            order.status = "novo";
            order.userEmail = User.Identity.Name;
            order.peso = 0;
            order.frete = 0;
            order.preco = 0;
            order.dataPedido = DateTime.Now;

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [Authorize]
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            if (User.IsInRole("USER"))
            {
                String usuario = User.Identity.Name;
                Order order = db.Orders.Where(e => e.userEmail == usuario && e.Id == id).FirstOrDefault();

                if (order == null)
                {
                    return NotFound();
                }

                db.Orders.Remove(order);
                db.SaveChanges();

                return Ok(order);
            }

            Order orderAdmin = db.Orders.Find(id);

            if (orderAdmin == null)
            {
                return NotFound();
            }

            db.Orders.Remove(orderAdmin);
            db.SaveChanges();

            return Ok(orderAdmin);
        }

        //api/Orders/fecharpedido/5
        [Authorize]
        [ResponseType(typeof(Order))]
        [HttpGet]
        [Route("closeOrder")]
        public IHttpActionResult CloseOrder(int id)
        {
            Order order = db.Orders.Where(p => p.Id == id).FirstOrDefault();

            if (order == null)
                return Ok("O pedido não existe");
            if ((order.userEmail == User.Identity.Name) || (User.IsInRole("ADMIN")))
            {
                if (order.frete == (decimal)0.00)
                    return Ok("ERRO - É necessário calcular o valor do frete");

                order.status = "fechado";
                db.SaveChanges();
                return Ok("Pedido fechado!");
            }
            else
            {
                return Ok("Acesso não autorizado.");
            }
        }

        [Authorize]
        [ResponseType(typeof(string))]
        [HttpGet]
        [Route("frete")]
        public IHttpActionResult consultaFrete(int idPedido)
        {
            decimal pesototal = 0, alturaTotal = 0, largura = 0, comprimento = 0, diametro = 0, precoTotal = 0, valorFrete = 0;
            String CEPDestino, prazoEntrega;
            cResultado resultado;
            Order order = db.Orders.Where(p => p.Id == idPedido).FirstOrDefault();

            if (order == null)
                return Ok("O pedido não existe. Favor fornecer um ID válido.");

            if (order.OrderItems.Count == 0)
                return Ok("Pedido sem itens.");

            if (order.status != "novo")
                return Ok("O pedido já foi modificado.");

            if ((order.userEmail == User.Identity.Name) || (User.IsInRole("ADMIN")))
            {

                CRMRestClient crmClient = new CRMRestClient();
                try
                {
                    Customer customer = crmClient.GetCustomerByEmail(User.Identity.Name);
                    CEPDestino = customer.zip;
                }
                catch
                {
                    return Ok("Não foi possivel acessar o serviço. Verifique se o e-mail usado está cadastrado no CRM.");
                }

                for (int cont = 0; cont < order.OrderItems.Count; cont++)
                {
                    alturaTotal += decimal.Parse(order.OrderItems.ElementAt(cont).Product.altura);
                    precoTotal += (Convert.ToDecimal(order.OrderItems.ElementAt(cont).quantidade, CultureInfo.InvariantCulture) * decimal.Parse(order.OrderItems.ElementAt(cont).Product.preco));
                    pesototal += (Convert.ToDecimal(order.OrderItems.ElementAt(cont).quantidade, CultureInfo.InvariantCulture) * decimal.Parse(order.OrderItems.ElementAt(cont).Product.peso));

                    if (Convert.ToDecimal(order.OrderItems.ElementAt(cont).Product.largura, CultureInfo.InvariantCulture) > largura)
                        largura = Convert.ToDecimal(order.OrderItems.ElementAt(cont).Product.largura, CultureInfo.InvariantCulture);

                    if (Convert.ToDecimal(order.OrderItems.ElementAt(cont).Product.comprimento, CultureInfo.InvariantCulture) > comprimento)
                        comprimento = (Convert.ToDecimal(order.OrderItems.ElementAt(cont).quantidade, CultureInfo.InvariantCulture) * Convert.ToDecimal(order.OrderItems.ElementAt(cont).Product.comprimento, CultureInfo.InvariantCulture));

                    diametro = Convert.ToDecimal(order.OrderItems.ElementAt(cont).Product.diametro, CultureInfo.InvariantCulture);
                }

                CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
                try
                {
                    resultado = correios.CalcPrecoPrazo("", "", "40010", "37540000", CEPDestino, pesototal.ToString(), 1, comprimento, alturaTotal, largura, diametro, "N", 0, "S");
                    prazoEntrega = resultado.Servicos.ElementAt(0).PrazoEntrega;
                }
                catch
                {
                    return Ok("Não foi possivel acessar o serviço dos Correios.");

                }

                if (resultado.Servicos[0].Erro.Equals("0"))
                {
                    NumberFormatInfo nfi = new CultureInfo("pt-BR", false).NumberFormat;
                    //valorFrete = decimal.Parse(resultado.Servicos.Single().Valor, nfi);
                    valorFrete = decimal.Parse(resultado.Servicos[0].Valor, nfi);

                    int prazo = int.Parse(resultado.Servicos.Single().PrazoEntrega);

                    DateTime atual = DateTime.Now;

                    atual = atual.AddDays(prazo);

                    order.peso = pesototal;
                    order.frete = valorFrete;
                    order.preco = precoTotal;
                    order.dataEntrega = atual;

                    db.SaveChanges();

                    return Ok("Frete calculado - Pedido: " + idPedido + ", Preço do frete: R$ "+ resultado.Servicos.Single().Valor + ", Prazo: " + resultado.Servicos.Single().PrazoEntrega + " (dias) )");
                }
                else
                {
                    return BadRequest("Erro: " + resultado.Servicos[0].Erro + "-" + resultado.Servicos[0].MsgErro);
                }

            }
            else
            {
                return Ok("Acesso não autorizado.");
            }
        }

        [ResponseType(typeof(string))]
        [HttpGet]
        [Route("cep")]
        public IHttpActionResult ObtemCEP()
        {
            CRMRestClient crmClient = new CRMRestClient();
            Customer customer = crmClient.GetCustomerByEmail(User.Identity.Name);
            if (customer != null)
            {
                return Ok(customer.zip);
            }
            else
            {
                return BadRequest("Falha ao consultar o CRM");
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }
    }
}