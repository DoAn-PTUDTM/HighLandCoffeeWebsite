using HighLandCoffeeWebsite.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace HighLandCoffeeWebsite.Services
{
    public class AdminService
    {
        public string conStr = "Data Source=TUYETCHINH;Initial Catalog=HIGHLANDCOFFEE;PersistSecurityInfo=True;User ID=sa;Password=123456;TrustServerCertificate=True";
        public List<Admin_Product> ExcuteSQL()
        {
            List<Admin_Product> productList = new List<Admin_Product>();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = conStr;
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                string selectStr = "SELECT * FROM PRODUCTS";
                SqlCommand cmd = new SqlCommand(selectStr, connection);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    int id = int.Parse(rdr["PRODUCTID"].ToString());
                    string name = rdr["NAME"].ToString();
                    string description = rdr["DESCRIPTION"].ToString();
                    decimal price = decimal.Parse(rdr["PRICE"].ToString());
                    string img = rdr["ImageUrl"].ToString();
                   // string createdAt = rdr["CREATED_AT_OF_PROD"].ToString().Substring(0, 10);

                    //DateTime updateDate = new DateTime();

                    //if (!string.IsNullOrEmpty(rdr["UPDATED_AT_OF_PROD"].ToString()))
                    //{
                    //    string updatedAt = rdr["UPDATED_AT_OF_PROD"].ToString().Substring(0, 10);
                    //    updateDate = DateTime.ParseExact(updatedAt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //}

                    //DateTime createDate = DateTime.ParseExact(createdAt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    int typeID = int.Parse(rdr["CATEGORYID"].ToString());

                    Admin_Product product = new Admin_Product(id, name, description, price, img, typeID);
                    productList.Add(product);
                }
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return productList;
        }

        public void AddSql(string name, string description, decimal price, string img, int typeID)
        {

            using (SqlConnection connection = new SqlConnection(conStr))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                string selectStr = "SET DATEFORMAT DMY;INSERT INTO PRODUCTS (NAME, DESCRIPTION, PRICE, ImageUrl, CATEGORYID)" +
                    "VALUES (N'" + name + "' , N'" + description + "', " + price + ", N'" + img + "', '" + typeID + "')";
                SqlCommand cmd = new SqlCommand(selectStr, connection);
                cmd.ExecuteNonQuery();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }

        }

        public void DeleteSql(int id)
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                string selectStr = "DELETE PRODUCTS WHERE PRODUCTID = '" + id + "'";
                SqlCommand cmd = new SqlCommand(selectStr, connection);
                cmd.ExecuteNonQuery();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public bool checkProductInCart(int id)
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                string selectStr = "SELECT COUNT(*) FROM ShoppingCart JOIN PRODUCTS ON PRODUCTS.PRODUCTID = ShoppingCart.PRODUCTID WHERE PRODUCTS.PRODUCTID = " + id;
                SqlCommand cmd = new SqlCommand(selectStr, connection);
                int count = (int)cmd.ExecuteScalar();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
                return count > 0;
            }
        }

    }
}