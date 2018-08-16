

namespace Sales.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common.Models;
    using Newtonsoft.Json;

    // haremos un método genérico que nos sirve para consumir de cualquier servicio API, y nos va a servir para consumir de cualquier lista.
    public class ApiService
    {
        // La clase Response, como es una clase que usaremos transversalmente, no la crearemos aquí, sino que la crearemos en Sales.Common.Models
        public async Task<Response> GetList <T> (string urlBase, string prefix, string controller)
        {
            try
            {
                var cliente = new HttpClient();
                cliente.BaseAddress = new Uri(urlBase);
                //var url = string.Format("{0}{1}", prefix,controller);
                var url = $"{prefix}{controller}"; // Esto concatena. Es equivalente al String.format
                var response = await cliente.GetAsync(url);
                var answer = await response.Content.ReadAsStringAsync(); // Aqui tenemos todo el json, pero en formato string. Hay que desserializarlo.
                if (!response.IsSuccessStatusCode) 
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = answer, // Que muestre lo que leyó en la variable answer
                    };
                }

                var list = JsonConvert.DeserializeObject<List<T>>(answer); // T sería la clase genérica. Aqui convertimos el string json, en una lista de objetos T
                return new Response
                {
                    IsSuccess = true,
                    Message = "",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

    }
}
