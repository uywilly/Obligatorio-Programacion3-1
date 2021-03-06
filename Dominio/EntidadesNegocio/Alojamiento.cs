﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;
using System.Data;
using System.Data.SqlClient;

namespace Dominio.EntidadesNegocio
{
    public class Alojamiento : IEntity
    {
        #region Properties
        public int Id { get; set; }
        public string Tipo { get; set; }
        public Ubicacion Ubicacion { get; set;}
        public List<RangoPrecio> Precios_temporada { get; set; }

        public string Mostrar
        {
            get {
                string pt = "";
                foreach(RangoPrecio unR in Precios_temporada)
                {
                    pt += unR.ToString() + "\n";
                }
                return this.ToString() + "--" + this.Ubicacion.ToString() + "--" + pt; }
        }
        #endregion

        #region Cadenas de comando para ACTIVE RECORD
        private string cadenaInsert = "INSERT INTO Alojamiento (tipo,idUbicacion) VALUES (@tipo,@idUbicacion); SELECT CAST(SCOPE_IDENTITY() AS INT);";
        private string cadenaUpdate = "UPDATE  Alojamiento SET tipo=@tipo WHERE id=@id";
        private string cadenaDelete = "DELETE  Alojamiento WHERE id=@id";
        #endregion

        #region Métodos ACTIVE RECORD
        public bool Add()
        {
            if (this.Validar())
            {
                SqlConnection cn = BdSQL.Conectar();
                SqlTransaction trn = null;
                try
                {
                    SqlCommand cmd = new SqlCommand(cadenaInsert, cn);

                    // acá va el resto de parametros que vamos a insertar...
                    cmd.Parameters.AddWithValue("@tipo", this.Tipo);
                    cmd.Parameters.AddWithValue("@idUbicacion", this.Ubicacion.Id);
                    //abrimos la coneccion
                    cn.Open();

                    //iniciamos la transaccion
                    trn = cn.BeginTransaction();
                    cmd.Transaction = trn;
                    int idAlojamiento = Convert.ToInt32(cmd.ExecuteScalar());
                    this.Id = idAlojamiento;
                    cmd.CommandText = "INSERT INTO RangoPrecio (fecha_ini,fecha_fin,variacion_precio,id_alojamiento) VALUES (@fecha_inicio,@fecha_fin,@variacion_precio,@id_alojamiento)";
                    foreach (RangoPrecio unR in this.Precios_temporada)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@id_alojamiento", idAlojamiento);
                        cmd.Parameters.AddWithValue("@fecha_inicio", unR.Fecha_inicio);
                        cmd.Parameters.AddWithValue("@fecha_fin", unR.Fecha_fin);
                        cmd.Parameters.AddWithValue("@variacion_precio", unR.Variacion_precio);
                        cmd.ExecuteNonQuery();
                    }
                    // Agregar la ubicacion 
                    cmd.CommandText = "INSERT INTO Ubicacion (ciudad,barrio,dirLinea1,dirLinea2) VALUES (@ciudad,@barrio,@dirLinea1,@dirLinea2); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ciudad", this.Ubicacion.Ciudad);
                    cmd.Parameters.AddWithValue("@barrio", this.Ubicacion.Barrio);
                    cmd.Parameters.AddWithValue("@dirLinea1", this.Ubicacion.DireccionLinea1);
                    cmd.Parameters.AddWithValue("@dirLinea2", this.Ubicacion.DireccionLinea2);
                    // me guardo el id de la ubicación insertada
                    int idUbicacion = Convert.ToInt32(cmd.ExecuteScalar());
                    this.Ubicacion.Id = idUbicacion;
                    // me agrego como parametro esa ubicacion
                    
                    // le hago un update al alojamiento con el idUbicacion(foreign key)..
                    cmd.CommandText = "UPDATE Alojamiento SET idUbicacion = @idUbicacion WHERE id = @id_alojamiento;";
                    cmd.Parameters.AddWithValue("@idUbicacion", idUbicacion);
                    cmd.Parameters.AddWithValue("@id_alojamiento", this.Id);
                    cmd.ExecuteNonQuery();
                    trn.Commit();
                    cmd.Parameters.Clear();
                    return true;

                }//fin del try
                catch (Exception ex)
                {
                    //falta hacer algo con la excepcion
                    BdSQL.LoguearError(ex.Message + "No se pudo agregar el Alojamiento");
                    trn.Rollback();
                    return false;

                }//fin del catch
                finally
                {
                    trn.Dispose();
                    trn = null;
                    cn.Close();
                    cn.Dispose();
                }
            }
            else
            {
                return false;
            }
        }
        public bool Update()
        {
            if (this.Validar())
            {
                using (SqlConnection cn = BdSQL.Conectar())
                {
                    using (SqlCommand cmd = new SqlCommand(cadenaUpdate, cn))
                    {
                        cmd.Parameters.AddWithValue("@tipo", this.Tipo);
                        //cmd.Parameters.AddWithValue("@cupo_max", this.Cupo_max);
                        cmd.Parameters.AddWithValue("@id", this.Id);
                        cn.Open();
                        int afectadas = cmd.ExecuteNonQuery();
                        return afectadas == 1;
                    }
                }
            }
            return false;
        }
        public bool Delete()
        {
            using (SqlConnection cn = BdSQL.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand(cadenaDelete, cn))
                {
                    cmd.Parameters.AddWithValue("@id", this.Id);
                    cn.Open();
                    int afectadas = cmd.ExecuteNonQuery();
                    return afectadas == 1;
                }
            }
        }
        public void Load(IDataRecord dr)
        {
            if (dr != null)
            {
                this.Tipo = dr["tipo"].ToString();               
                this.Ubicacion = null;
                this.Precios_temporada = null;
            }
        }
        public void loadRangoPrecio(RangoPrecio unR, IDataRecord dr)
        {
            if (dr != null)
            {
                unR.Fecha_inicio = dr.GetDateTime(dr.GetOrdinal("fecha_ini"));
                unR.Fecha_fin = dr.GetDateTime(dr.GetOrdinal("fecha_fin"));
                unR.Variacion_precio = dr.GetDecimal(dr.GetOrdinal("variacion_precio"));
                unR.Id = Convert.ToInt32(dr["id"]);
            }
        }
        #endregion

        #region Validaciones
        public bool Validar() // esto es cualquier cosa :)
        {
            return this.Tipo.Length >= 3;
        }
        #endregion

        #region Redefiniciones de object
        public override string ToString()
        {
            return this.Id + " - Tipo: " + this.Tipo ;
        }
        #endregion
    }
}
