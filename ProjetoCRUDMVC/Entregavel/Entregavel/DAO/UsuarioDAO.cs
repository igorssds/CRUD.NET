using Azure;
using Entregavel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Entregavel.DAO
{
    public static class UsuarioDAO 
    {
        public static void InserirUsuario(Usuario user)
        {
            if (string.IsNullOrWhiteSpace(user.Nome) || string.IsNullOrWhiteSpace(user.Email))
            {
                return;
            }

            using (SqlConnection conn = SqlConn.Abrir())
            {

                using (SqlCommand cmd = new SqlCommand("INSERT INTO USUARIO (Nome, Email) OUTPUT INSERTED.Id VALUES (@nome, @email)", conn))
                {
                    cmd.Parameters.AddWithValue("@nome", user.Nome);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    user.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public static List<Usuario> Listar()
        {
            using (SqlConnection conn = SqlConn.Abrir())
            {
                using (SqlCommand cmd = new SqlCommand("select id,nome, email from usuario", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Usuario> lista = new List<Usuario>();

                        while (reader.Read() == true)
                        {
                            lista.Add(new Usuario()
                            {
                                Id = reader.GetInt32(0),
                                Nome = reader.GetString(1),
                                Email = reader.GetString(2)
                            });
                        }
                        return lista;
                    }
                }
            }  
        }


        public static void Excluir(int id)
        {
            using (SqlConnection conn =SqlConn.Abrir())
            {

                using (SqlCommand cmd = new SqlCommand("DELETE FROM Usuario WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public static Usuario BuscarPorId(int id)
        {
            Usuario usuario;
            using (SqlConnection conn = SqlConn.Abrir())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT Id, Nome, Email FROM Usuario WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if(reader.Read() == true)
                        {
                            string nome = reader.GetString(1);
                            string email = reader.GetString(2);

                            usuario = new Usuario();
                            usuario.Id = id;
                            usuario.Nome = nome;
                            usuario.Email = email;
                        }
                        else
                        {
                            usuario = null;
                        }

                    }
                }

            }
                return usuario;
        }

        public static void Editar(Usuario user)
        {
            if (string.IsNullOrWhiteSpace(user.Nome) || string.IsNullOrWhiteSpace(user.Email))
            {
                return;
            }

            using (SqlConnection conn = SqlConn.Abrir())
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE Usuario SET Nome=@nome, Email=@email WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@nome", user.Nome);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@id", user.Id);

                    cmd.ExecuteNonQuery();
                }
            }
            
        }

        
    }
}