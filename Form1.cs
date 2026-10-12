using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace parcial2
{
    public partial class Form1 : Form
    {
        string cadenaConexion =
            @"Data Source=(localdb)\MSSQLLocalDB;
              Initial Catalog=ParcialProgramacion1;
              Integrated Security=True;";

        int IdProductoSeleccionado = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void ProbarConexion()
        {
            using (SqlConnection conexion =
                   new SqlConnection(cadenaConexion))
            {
                try
                {
                    conexion.Open();
                    MessageBox.Show(
                        "Conexión exitosa con SQL Server.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error de conexión: " + ex.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ProbarConexion();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string sql = @"INSERT INTO dbo.Productos
                   (Nombre, Categoria, Precio, Existencias)
                   VALUES
                   (@Producto, @Categoria, @Precio, @Existencias)";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", txtProducto.Text);
                    cmd.Parameters.AddWithValue("@Categoria", txtCategoria.Text);
                    cmd.Parameters.AddWithValue("@Precio", decimal.Parse(txtPrecio.Text));
                    cmd.Parameters.AddWithValue("@Existencias", int.Parse(txtExistencias.Text));

                    try
                    {
                        conexion.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Producto guardado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al guardar: " + ex.Message);
                    }
                }
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM dbo.Productos";

            using (SqlConnection conexion =
                   new SqlConnection(cadenaConexion))
            {
                using (SqlDataAdapter adaptador =
                       new SqlDataAdapter(sql, conexion))
                {
                    DataTable tabla = new DataTable();

                    adaptador.Fill(tabla);

                    //Para mostrar la tabla
                    dataGridView1.DataSource = tabla;
                }
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            decimal precio;

            if (!decimal.TryParse(txtPrecio.Text, out precio))
            {
                MessageBox.Show("Ingresa un precio válido.");
                return;
            }

            int existencias;

            if (!int.TryParse(txtExistencias.Text, out existencias))
            {
                MessageBox.Show("Ingresa una cantidad válida.");
                return;
            }

            string sql = @"UPDATE dbo.Productos
                   SET Nombre = @Nombre,
                       Categoria = @Categoria,
                       Precio = @Precio,
                       Existencias = @Existencias
                   WHERE IdProducto = @IdProducto";

            using (SqlConnection conexion =
                   new SqlConnection(cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", txtProducto.Text);
                    cmd.Parameters.AddWithValue("@Categoria", txtCategoria.Text);
                    cmd.Parameters.AddWithValue("@Precio", txtPrecio.Text); 
                    cmd.Parameters.AddWithValue("@Existencias", txtExistencias.Text);
                    cmd.Parameters.AddWithValue("@IdProducto", IdProductoSeleccionado);

                    conexion.Open();

                    int filas = cmd.ExecuteNonQuery();

                    MessageBox.Show(
                        filas > 0 ? "Producto actualizado." :
                                    "No se encontró el producto.");
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            DialogResult respuesta = MessageBox.Show(
        "¿Deseas eliminar este producto?",
        "Confirmar eliminación",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            string sql = "DELETE FROM dbo.Productos WHERE IdProducto = @IdProducto";

            using (SqlConnection conexion =
                   new SqlConnection(cadenaConexion))
            using (SqlCommand cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@IdProducto", IdProductoSeleccionado);

                conexion.Open();
                int filas = cmd.ExecuteNonQuery();

                MessageBox.Show(
                    filas > 0 ? "Producto eliminado." :
                                "No se encontró el producto.");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtProducto.Clear();
            txtCategoria.Clear();
            txtPrecio.Clear();
            txtExistencias.Clear();

            txtProducto.Focus();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

            IdProductoSeleccionado = Convert.ToInt32(
                fila.Cells["IdProducto"].Value);

            txtProducto.Text = fila.Cells["Producto"].Value.ToString();
            txtCategoria.Text = fila.Cells["Categoria"].Value.ToString();
            txtPrecio.Text = fila.Cells["Precio"].Value.ToString();
            txtExistencias.Text = fila.Cells["Existencias"].Value.ToString();
        }
    }
}

