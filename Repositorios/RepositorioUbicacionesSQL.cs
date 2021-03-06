﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Dominio.EntidadesNegocio;
using Dominio.InterfacesRepositorio;
using System.Data.SqlClient;
using Utilidades;

namespace Repositorios
{
    public class RepositorioUbicacionesSQL : IRepositorioUbicaciones
    {
        public List<Ubicacion> FindAll()
        {
            string cadenaFindAll = "SELECT id, ciudad, barrio, dirLinea1, dirLinea2 FROM Ubicacion";
            List<Ubicacion> listaUbicaciones = new List<Ubicacion>();
            using (SqlConnection cn = BdSQL.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand(cadenaFindAll, cn))
                {
                    cn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Ubicacion unaU = new Ubicacion();
                            unaU.Load(reader);
                            if (unaU.Validar())
                                listaUbicaciones.Add(unaU);
                        }
                    }
                }
            }
            return listaUbicaciones;
        }

        public Ubicacion FindById(int id)
        {
            string cadenaFind = "SELECT ciudad, barrio, dirLinea1, dirLinea2 FROM Ubicacion WHERE id = @id";
            Ubicacion unaU = null;
            using (SqlConnection cn = BdSQL.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand(cadenaFind, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader != null && reader.Read())
                    {
                        unaU = new Ubicacion();
                        unaU.Load(reader);
                        unaU.Id = id;
                    }
                }
            }
            return unaU;
        }

        public bool Delete(int id)
        {
            Ubicacion u = this.FindById(id);
            return (u != null && u.Delete());

        }

        public bool Update(Ubicacion obj)
        {
            return obj != null && obj.Update();
        }
    }
}
