using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using HydraFramework.Modulos;
using static HydraFramework.Modulos.Carrega;
using static HydraFramework.Modulos.Manipula;

namespace HydraFramework
{
    internal class BaseHydra
    {
        private string Conexao { get; set; } = "";

        public BaseHydra(string conexao)
        {
            Conexao = conexao;
        }

        public SqlConnection AbrirBanco()
        {
            SqlConnection conexao = new SqlConnection(Conexao);
            conexao.Open();

            return conexao;
        }

        public void FecharBanco(SqlConnection conexao)
        {
            if (conexao.State == ConnectionState.Open)
            {
                conexao.Close();
            } 
        }

        public object Save(object entidade)
        {
            string stringConsulta = "";
            bool pkIdentidade = false;
            bool novoItem;

            Type tipo = entidade.GetType();
            HydraParameters hydraParametros = new HydraParameters();

            PropertyInfo PK = tipo.GetProperties().Where(x => Valida.PrimaryKey(x) != null).FirstOrDefault();
            PropertyInfo[] colunas = tipo.GetProperties().Where(x => Valida.Coluna(x) != null).ToArray();
            Dictionary<PropriedadePK, object> propriedadesPK = Carrega.InfoPrimaryKey(entidade, tipo, hydraParametros, PK);
            List<string> NomeColunas = Carrega.Parametros(entidade, tipo, hydraParametros, colunas);

            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader dadosTabela;

            if (propriedadesPK != null)
            {
                string valorPK = propriedadesPK[PropriedadePK.Valor].ToString();
                string nomePK = propriedadesPK[PropriedadePK.Nome].ToString();
                pkIdentidade = bool.Parse(propriedadesPK[PropriedadePK.Identidade].ToString());

                string consultaObjeto = "";
                Manipula.Consulta(out consultaObjeto, tipo, TipoConsulta.Select, 1, nomePK, condicoes: $"WHERE {nomePK} = {valorPK}");

                try
                {
                    sqlConnection = AbrirBanco();

                    using (comando = new SqlCommand(consultaObjeto, sqlConnection))
                    {
                        using (dadosTabela = comando.ExecuteReader())
                        {
                            if (dadosTabela.Read())
                            {
                                novoItem = false;
                            }
                            else
                            {
                                novoItem = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    FecharBanco(sqlConnection);
                    throw ex;
                }

                if (novoItem == true)
                {
                    if (pkIdentidade == true)
                    {
                        stringConsulta = Insert(tipo, NomeColunas, nomePK);
                    }
                    else
                    {
                        stringConsulta = Insert(tipo, NomeColunas, nomePK, false);
                    }
                }
                else
                {
                    if (pkIdentidade == true)
                    {
                        stringConsulta = Update(tipo, NomeColunas, nomePK, valorPK);
                    }
                    else
                    {
                        stringConsulta = Update(tipo, NomeColunas, nomePK, valorPK, false);
                    }

                }

                try
                {
                    using (comando = new SqlCommand(stringConsulta, sqlConnection))
                    {
                        Popula.Parametros(hydraParametros, comando, pkIdentidade);

                        using (dadosTabela = comando.ExecuteReader())
                        {
                            Popula.Objeto(dadosTabela, entidade, tipo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    FecharBanco(sqlConnection);
                }
            }
            else
            {
                entidade = new object();
            }

            return entidade;
        }

        public bool Delete(object entidade)
        {
            string stringConsulta = "";
            bool retorno = false;

            Type tipo = entidade.GetType();
            HydraParameters hydraParametros = new HydraParameters();

            PropertyInfo PK = tipo.GetProperties().Where(x => Valida.PrimaryKey(x) != null).FirstOrDefault();
            Dictionary<PropriedadePK, object> propriedadesPK = Carrega.InfoPrimaryKey(entidade, tipo, hydraParametros, PK);
            
            if (propriedadesPK != null)
            {
                string valorPK = propriedadesPK[PropriedadePK.Valor].ToString();
                string nomePK = propriedadesPK[PropriedadePK.Nome].ToString();

                stringConsulta = Manipula.Delete(tipo, nomePK, valorPK);

                SqlConnection sqlConnection = new SqlConnection();

                try
                {
                    sqlConnection = AbrirBanco();

                    using (SqlCommand comando = new SqlCommand(stringConsulta, sqlConnection))
                    {
                        using (SqlDataReader dadosTabela = comando.ExecuteReader())
                        {
                            retorno = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    FecharBanco(sqlConnection);
                }
            }
            else
            {
                retorno = false;
            }

            return retorno;
        }

        public bool Consulta(string stringSQL, HydraParameters parametros)
        {
            bool retorno = false;
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = AbrirBanco();

                using (SqlCommand comando = new SqlCommand(stringSQL, sqlConnection))
                {
                    Popula.Parametros(parametros, comando, false);

                    comando.ExecuteReader();
                }

                retorno = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FecharBanco(sqlConnection);
            }

            return retorno;
        }

        public List<T> ConsultaEmLista<T>(string stringSQL, HydraParameters parametros = null) 
        {
            List<T> lista = new List<T>();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = AbrirBanco();

                using (SqlCommand comando = new SqlCommand(stringSQL, sqlConnection))
                {
                    Popula.Parametros(parametros, comando, false);

                    using (SqlDataReader dadosTabela = comando.ExecuteReader())
                    {
                        lista = Popula.Lista<T>(dadosTabela);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FecharBanco(sqlConnection);
            }

            return lista;
        }
            
        public T ConsultaEmEntidade<T>(string stringSQL, HydraParameters parametros = null)
        {
            T entidade;
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = AbrirBanco();

                using (SqlCommand comando = new SqlCommand(stringSQL, sqlConnection))
                {
                    Popula.Parametros(parametros, comando, false);

                    using (SqlDataReader dadosTabela = comando.ExecuteReader())
                    {
                        
                        entidade = Popula.Entidade<T>(dadosTabela);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FecharBanco(sqlConnection);
            }

            return entidade;
        }

        public List<HydraTuple<T1, T2>> ConsultaEmLista<T1, T2>(string stringSQL, HydraParameters parametros = null)
        {
            List<HydraTuple<T1, T2>> lista = new List<HydraTuple<T1, T2>>();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = AbrirBanco();

                using (SqlCommand comando = new SqlCommand(stringSQL, sqlConnection))
                {
                    Popula.Parametros(parametros, comando, false);

                    using (SqlDataReader dadosTabela = comando.ExecuteReader())
                    {
                        lista = Popula.ListaDuasTuple<T1, T2>(dadosTabela);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FecharBanco(sqlConnection);
            }

            return lista;
        }

        public List<HydraTuple<T1, T2, T3>> ConsultaEmLista<T1, T2, T3>(string stringSQL, HydraParameters parametros = null)
        {
            List<HydraTuple<T1, T2, T3>> lista = new List<HydraTuple<T1, T2, T3>>();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = AbrirBanco();

                using (SqlCommand comando = new SqlCommand(stringSQL, sqlConnection))
                {
                    Popula.Parametros(parametros, comando, false);

                    using (SqlDataReader dadosTabela = comando.ExecuteReader())
                    {
                        lista = Popula.ListaTresTuple<T1, T2, T3>(dadosTabela);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FecharBanco(sqlConnection);
            }

            return lista;
        }

        public DataTable ConsultaEmDataTable(string stringSQL)
        {
            DataTable dataTable = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = AbrirBanco();

                using (SqlCommand comando = new SqlCommand(stringSQL, sqlConnection))
                {
                    using (SqlDataReader dadosTabela = comando.ExecuteReader())
                    {
                        dataTable = Popula.DataTable(dadosTabela);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FecharBanco(sqlConnection);
            }

            return dataTable;
        }
    }
}