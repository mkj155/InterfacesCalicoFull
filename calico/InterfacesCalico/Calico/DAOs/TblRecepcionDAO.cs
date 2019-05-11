using Calico.persistencia;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.DAOs
{
    class TblRecepcionDAO : CommonDAO, Dao<tblRecepcion>
    {
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public DbSet<tblRecepcion> FindAll()
        {
            throw new NotImplementedException();
        }

        public tblRecepcion FindById(int id)
        {
            throw new NotImplementedException();
        }

        public void Save(tblRecepcion obj)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                try
                {
                    context.tblRecepcion.Add(obj);
                    context.SaveChanges();
                }

                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.Error.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
        }

        public void Update(tblRecepcion obj)
        {
            throw new NotImplementedException();
        }

        public int CountByFields(String emplaz, String alm, String cod, String numero)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                int count = context.tblRecepcion
               .Where(x => x.recc_emplazamiento == emplaz &&
                      x.recc_almacen == alm &&
                      x.recc_trec_codigo == cod &&
                      x.recc_numero == numero)
               .Count<tblRecepcion>();

                count += context.tblHistoricoRecepcion
               .Where(x => x.hrec_emplazamiento == emplaz &&
                      x.hrec_almacen == alm &&
                      x.hrec_trec_codigo == cod &&
                      x.hrec_numero == numero)
               .Count<tblHistoricoRecepcion>();

                return count;
            }
        }

        public void examplePersist(String empl, String alm, String cod, String num, String compañia)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                
                /* Cabecera */
                tblRecepcion recept = new tblRecepcion();
                recept.recc_proc_id = 1;
                recept.recc_emplazamiento = empl;
                recept.recc_almacen = alm;
                recept.recc_trec_codigo = cod;
                recept.recc_numero = num;
                recept.recc_fechaEmision = DateTime.Now;
                recept.recc_fechaEntrega = DateTime.Now;
                recept.recc_proveedor = "Proveedor";

                /* Detalle */
                tblRecepcionDetalle detalle = new tblRecepcionDetalle();
                detalle.recd_proc_id = 1;
                detalle.recd_compania = compañia;
                detalle.recd_producto = "Producto_rest";
                detalle.recd_lote = "lote";
                detalle.recd_fechaVencimiento = DateTime.Now;
                detalle.recd_cantidad = 12;
                detalle.recd_numeroPedido = "1";
                detalle.tblRecepcion = recept;

                recept.tblRecepcionDetalle.Add(detalle);

                Save(recept);
            }
        }
    }
}
