using FichaEpid;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_Final_5to_Semestre_Web
{
    public partial class frmNuevaImagen : System.Web.UI.Page
    {
        protected static Funciones fn;
        protected static PaginaMaestra mst;
        protected static string DireccionMaestra = @"C:\Users\JonathanY\Desktop\Imagenes";
        protected static int contadorveces = 1;
        protected static string palabranueva;
        protected static string palabraantigua;
        protected static int contadorNuevaCorrecta = 1;
        protected void Page_Load(object sender, EventArgs e)
        {
            fn = new Funciones();
            mst = (PaginaMaestra)Master;
            if (!IsPostBack)
            {
                Traducir();
                LlenarHistorial();
                pnlVista.Visible = false;
            }
        }
        protected void Traducir()
        {
            btnsubir.Text = "Subir imagen";
            txtCapchaCon.Text = "";
            txtCapchaNoCon.Text = "";
            lbltituloCapcha.Text = "Captcha Parte 1 de 3";
        }

        protected void btnsubir_Click(object sender, EventArgs e)
        {
            string palabracomun = txtCapchaCon.Text;
            string palabranocomun = txtCapchaNoCon.Text;

            string sql = "SELECT Campo_Informacion info FROM dbo.Captchas WHERE Id_Captcha=1 AND Activo=1";
            string verdadero = fn.obtienePalabra(sql, "info");

            sql = "SELECT COUNT(*) conteo FROM dbo.Captchas WHERE Id_Captcha=2 AND Activo=1";
            int contadorPalabraNueva = fn.ObtenerEntero(sql, "conteo");
            string nuevoverdadero = "";
            if (contadorPalabraNueva > 0)
            {
                sql = "SELECT Campo_Informacion info FROM dbo.Captchas WHERE Id_Captcha=2 AND Activo=1";
                nuevoverdadero = fn.obtienePalabra(sql, "info");
            }

            if (contadorveces == 1)
            {
                if (verdadero == palabracomun)
                {
                    if (string.IsNullOrEmpty(nuevoverdadero))
                    {
                        palabranueva = palabranocomun;
                        contadorNuevaCorrecta++;
                        contadorveces++;
                        txtCapchaCon.Text = "";
                        txtCapchaNoCon.Text = "";
                        lbltituloCapcha.Text = "Captcha Parte 2 de 3";
                    }
                    else
                    {
                        if (nuevoverdadero == palabranocomun)
                        {
                            palabranueva = palabranocomun;
                            contadorveces++;
                            txtCapchaCon.Text = "";
                            txtCapchaNoCon.Text = "";
                            lbltituloCapcha.Text = "Captcha Parte 2 de 3";
                        }
                    }
                }
                else
                {
                    mst.MostrarMensaje("Verifique que este bien escrito");
                }
            }
            else if (contadorveces == 2)
            {
                if (verdadero == palabracomun)
                {
                    if (string.IsNullOrEmpty(nuevoverdadero))
                    {
                        if (palabranueva == palabranocomun)
                        {
                            contadorNuevaCorrecta++;
                        }
                        palabranueva = palabranocomun;
                        contadorveces++;
                        txtCapchaCon.Text = "";
                        txtCapchaNoCon.Text = "";
                        lbltituloCapcha.Text = "Captcha Parte 3 de 3";
                    }
                    else
                    {
                        if (nuevoverdadero == palabranocomun)
                        {
                            palabranueva = palabranocomun;
                            contadorveces++;
                            txtCapchaCon.Text = "";
                            txtCapchaNoCon.Text = "";
                            lbltituloCapcha.Text = "Captcha Parte 3 de 3";
                        }
                    }
                }
                else
                {
                    mst.MostrarMensaje("Verifique que este bien escrito");
                }
            }
            else if (contadorveces == 3)
            {
                if (verdadero == palabracomun)
                {
                    if (string.IsNullOrEmpty(nuevoverdadero))
                    {
                        if (palabranueva == palabranocomun && contadorNuevaCorrecta == 3)
                        {
                            sql = "INSERT INTO DBO.Captchas " +
                            "VALUES('2.jpg', 2, '" + Sesion.U + "', GETDATE(), 1, '"+palabranueva+"', 1)";
                            fn.ejecutarSQL(sql);
                        }
                    }
                    else
                    {
                        if (nuevoverdadero == palabranocomun)
                        {
                            palabranueva = palabranocomun;
                            contadorveces++;
                        }
                    }
                    if (flpDragAndDrop.HasFile == null)
                    {
                        mst.MostrarMensaje("No se ha seleccionado ningun archivo", 1);
                    }
                    else
                    {
                        string extension = Path.GetExtension(flpDragAndDrop.PostedFile.FileName);
                        string rutaFoto = Path.GetDirectoryName(flpDragAndDrop.PostedFile.FileName);
                        switch (extension.ToLower())
                        {

                            case ".jpg":
                                transferirFoto(rutaFoto);
                                break;
                            case "":
                                mst.MostrarMensaje("No se ha seleccionado ningun archivo", 1);
                                break;


                            default:
                                mst.MostrarMensaje("Extension Invalida", 1);

                                break;
                        }
                    }
                }
            }
        }

        protected void transferirFoto(string fotoactual)
        {
            DateTime dtFechaNombre = DateTime.Now;
            string nombreNuevoArchivo = "IMG_" + dtFechaNombre.ToString("yyyyMMddHHmmss") + ".jpg";

            string direccionFinal = Path.Combine(DireccionMaestra, nombreNuevoArchivo);


            flpDragAndDrop.SaveAs(direccionFinal);

            AgregarImagen(direccionFinal, 1, dtFechaNombre);
            AgregarImagen(direccionFinal, 2, dtFechaNombre);
            AgregarImagen(direccionFinal, 3, dtFechaNombre);
            AgregarImagen(direccionFinal, 4, dtFechaNombre);
            AgregarImagen(direccionFinal, 5, dtFechaNombre);
            AgregarImagen(direccionFinal, 6, dtFechaNombre);

            int id = nuevoRegistroImagen();
            AgregarRegistroBD(id, direccionFinal, dtFechaNombre);

            LlenarHistorial();

            mst.MostrarMensaje("Se ha ingresado exitosamente");
        }

        protected string Miniatura(DateTime dt, int numero)
        {
            return "IMG_" + dt.ToString("yyyyMMddHHmmss") + "_miniatura" + numero.ToString() + ".jpg";
        }
        protected void AgregarRegistroBD(int id, string direccion, DateTime dt)
        {
            try
            {
                string sql = "INSERT INTO dbo.Imagenes(Id_Imagen, Usuario, Fecha_Creacion, Activo, Imagen_1, Imagen_2, Imagen_3, Imagen_4, Imagen_5, Imagen_6) " +
                    "VALUES(" + id.ToString() + ",'" + Sesion.U + "',GETDATE(),1,'" + Miniatura(dt, 1) + "','" + Miniatura(dt, 2) + "','" + Miniatura(dt, 3) + "','" + Miniatura(dt, 4) + "','" + Miniatura(dt, 5) + "','" + Miniatura(dt, 6) + "')";
                fn.ejecutarSQL(sql);
            }
            catch (Exception ex)
            {
                mst.MostrarMensaje("Se ha encontrado el siguiente error:" + ex.Message, 3);
            }
        }
        protected int nuevoRegistroImagen()
        {
            string sql = "SELECT COUNT(*) conteo FROM dbo.Imagenes";
            int contador = fn.ObtenerEntero(sql, "conteo");
            if (contador > 0)
            {
                sql = "SELECT MAX(Id_Imagen)+1 conteo FROM dbo.Imagenes";
                return fn.ObtenerEntero(sql, "conteo");
            }
            return 1;
        }
        protected void AgregarImagen(string direccionFinal, int numero, DateTime dtFechaNombre)
        {
            Bitmap nb = new Bitmap(direccionFinal);
            int x = numero % 2 == 0 ? (int)nb.Width / 2 : 0;
            int y = 0;

            int tercios = (int)nb.Height / 3;
            string nombreArchivoMini = "IMG_" + dtFechaNombre.ToString("yyyyMMddHHmmss") + "_miniatura" + numero.ToString() + ".jpg";
            string direccionMiniFinal = Path.Combine(DireccionMaestra, nombreArchivoMini);

            if (numero == 3 || numero == 4)
            {
                y = tercios;
            }
            else if (numero == 5 || numero == 6)
            {
                y = (int)tercios * 2;
            }

            Bitmap mp = CropCenter(nb, (int)nb.Width / 2, tercios, x, y);
            mp.Save(direccionMiniFinal);
        }
        public Bitmap CropCenter(Bitmap src, int targetWidth, int targetHeight, int x, int y)
        {

            Rectangle area = new Rectangle(x, y, targetWidth, targetHeight);

            return src.Clone(area, src.PixelFormat);
        }
        protected void LlenarHistorial()
        {
            string sql = "SELECT Id_Imagen ID, Usuario 'Registrado Por', " +
            "Fecha_Creacion Fecha," +
            "CASE WHEN Imagen_1 IS NOT NULL THEN 'Si' ELSE 'No'  END NIT, CASE WHEN Imagen_2 IS NOT NULL THEN 'Si' ELSE 'No'  END Fecha, " +
            "CASE WHEN Imagen_3 IS NOT NULL THEN 'Si' ELSE 'No'  END 'Articulo 1', CASE WHEN Imagen_4 IS NOT NULL THEN 'Si' ELSE 'No'  END 'Cantidad 1', " +
            "CASE WHEN Imagen_5 IS NOT NULL THEN 'Si' ELSE 'No'  END 'Articulo 2', CASE WHEN Imagen_6 IS NOT NULL THEN 'Si' ELSE 'No'  END 'Cantidad 2' " +
            "FROM dbo.Imagenes " +
            "WHERE Usuario = '" + Sesion.U + "' AND Activo = 1";
            fn.LLenarGrid(sql, gvhistorial);
        }

        protected void gvhistorial_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "cmdSelect")
            {

                pnlVista.Visible = false;
            }
            else if (e.CommandName == "cmdDelete")
            {
                String miembro = gvhistorial.Rows[Int32.Parse(e.CommandArgument.ToString())].Cells[0].Text;
                string sql = "SELECT Imagen_1, Imagen_2, Imagen_3, Imagen_4, Imagen_5, Imagen_6 FROM dbo.Imagenes WHERE Activo = 1 AND Id_Imagen =" + miembro.ToString();
                DataTable dt = new DataTable();
                fn.LlenarDataTable(sql, dt);
                AsignarFoto(dt, 1);
                AsignarFoto(dt, 2);
                AsignarFoto(dt, 3);
                AsignarFoto(dt, 4);
                AsignarFoto(dt, 5);
                AsignarFoto(dt, 6);
            }
        }
        protected void AsignarFoto(DataTable dt, int numero)
        {

            string nombreFoto = Path.Combine(DireccionMaestra, dt.Rows[0][numero - 1].ToString());


            //File.Delete();
        }
    }
}