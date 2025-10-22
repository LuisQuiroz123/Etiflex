using System.Collections.Generic;

namespace EtiWeb.Helpers
{
    public static class MenuHelper
    {
        /// <summary>
        /// Devuelve la estructura del menú:
        /// Área -> Lista de controladores -> Lista de acciones
        /// </summary>
        public static Dictionary<string, List<ControllerMenu>> GetMenuStructure()
        {
            return new Dictionary<string, List<ControllerMenu>>
            {
                ["Default"] = new List<ControllerMenu>
                {
                    new ControllerMenu
                    {
                        Controller = "Home",
                        DisplayName = "Inicio",
                        Actions = new List<ActionMenu>
                        {
                            new ActionMenu { Action = "Index", DisplayName = "Página Principal" },
                            new ActionMenu { Action = "Privacy", DisplayName = "Política de Privacidad" }
                        }
                    }
                },

                ["ApiArea"] = new List<ControllerMenu>
                {
                    //new ControllerMenu
                    //{
                    //    Controller = "Orders",
                    //    DisplayName = "Ordenes de Prueba",
                    //    Actions = new List<ActionMenu>
                    //    {
                    //        new ActionMenu { Action = "Index", DisplayName = "Lista de Ordenes" },
                    //        //new ActionMenu { Action = "Details", DisplayName = "Detalle del Pedido" }
                    //    }
                    //},

                    new ControllerMenu
                    {
                        Controller = "Requests",
                        DisplayName = "Solicitud Etiquetas",
                        Actions = new List<ActionMenu>
                        {
                            new ActionMenu { Action = "Index", DisplayName = "Lista de Pedidos" },
                            //new ActionMenu { Action = "Details", DisplayName = "Detalle del Pedido" }
                        }
                    },

                    //new ControllerMenu
                    //{
                    //    Controller = "RequestFiles",
                    //    DisplayName = "Archivos",
                    //    Actions = new List<ActionMenu>
                    //    {
                    //        new ActionMenu { Action = "Index", DisplayName = "Lista de Archivos" },
                    //        new ActionMenu { Action = " ", DisplayName = "Etiquetas" }
                    //    }
                    //},

                            new ControllerMenu
                    {
                        Controller = "SalesOrders",
                        DisplayName = "Ordenes de Venta",
                        Actions = new List<ActionMenu>
                        {
                            new ActionMenu { Action = "Index", DisplayName = "Lista de Ordenes" }
                        }
                    }
                },
            };
        }

        /// <summary>
        /// Diccionario  para las áreas
        /// </summary>
        public static readonly Dictionary<string, string> AreaDisplayNames = new()
        {
            { "Default", "Inicio" },
            { "ApiArea", "API's" },           
            { "Admin", "Administración" }
        };
    }

    public class ControllerMenu
    {
        public string Controller { get; set; }
        public string DisplayName { get; set; }
        public List<ActionMenu> Actions { get; set; }
    }

    public class ActionMenu
    {
        public string Action { get; set; }
        public string DisplayName { get; set; }
    }
}