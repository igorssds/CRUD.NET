using Azure;
using Entregavel.DAO;
using Entregavel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Entregavel.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult Table()
        {
            return PartialView("_Table", UsuarioDAO.Listar());
        }

        public ActionResult Cadastro(Usuario user)
        {
            if (Request.Files.Count == 0)
            {
                return Json("Sem arquivos!", JsonRequestBehavior.AllowGet);
            }

            UsuarioDAO.InserirUsuario(user);

            AzureStorage.Upload(
                "web",
                user.Id + ".jpg",
                Request.Files[0].InputStream,
                "teste",
                "UseDevelopmentStorage=true");

            return Json("Sucesso!", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Excluir(int id)
        {
            UsuarioDAO.Excluir(id);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Editar(Usuario user)
        {
            UsuarioDAO.Editar(user);

            AzureStorage.Upload(
                "web",
                user.Id + ".jpg",
                Request.Files[0].InputStream,
                "teste",
                "UseDevelopmentStorage=true");
            return Json("Editado");

        }

        public ActionResult ModalNovo()
        {
            return PartialView("_ModalNovo");
        }

        public ActionResult ModalEditar(int id)
        {
            Usuario usuario = new Usuario();
            usuario = UsuarioDAO.BuscarPorId(id);
            return PartialView("_ModalEditar", usuario );
        }

        public ActionResult ModalExcluir(Usuario user)
        {
            return PartialView("_ModalExcluir", user);
        }
    }
}