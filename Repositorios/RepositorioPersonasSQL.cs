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
    public class RepositorioPersonasSQL : IRepositorioPersonas
    {
        public List<Persona> FindAll()
        {
            string cadenaFindAll = "SELECT nombre, apellido, ci, email FROM Persona";
            List<Persona> listaPersonas = new List<Persona>();
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
                            Persona unaP = new Persona();
                            unaP.Load(reader);
                            if (unaP.Validar())
                                listaPersonas.Add(unaP);
                        }
                    }
                }
            }
            return listaPersonas;
        }

        public Persona FindById(int id)
        {
            string cadenaFind = "SELECT nombre, apellido, ci, email FROM Persona WHERE id = @id";
            Persona unaP = null;
            using (SqlConnection cn = BdSQL.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand(cadenaFind, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader != null && reader.Read())
                    {
                        unaP = new Persona();
                        unaP.Load(reader);
                    }
                }
            }
            return unaP;
        }

        public bool Add(Persona obj)
        {
            return obj != null && obj.Add();
        }

        public bool Delete(int id)
        {
            //Persona p = this.FindById(id);
            //return (p != null && p.Delete());
            return true;

        }

        public bool Update(Persona obj)
        {
            //return obj != null && obj.Update();
            return true;
        }
    }
}
