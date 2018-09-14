

namespace Sales.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Common.Models;
    using Helpers;
    using Newtonsoft.Json;
    using Plugin.Connectivity;

    // haremos un método genérico que nos sirve para consumir de cualquier servicio API, y nos va a servir para consumir de cualquier lista.
    public class ApiService
    {

        /*
         * NOTA: 
         * La clase Task es la encargada de ejecutar una o varias tareas fuera del hilo que 
         * la ejecuta creando ésta un Thread propio para evitar congelar la interfaz del usuario.
         */

        public async Task<TokenResponse> GetToken(string urlBase, string username, string password)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var response = await client.PostAsync("Token",
                    new StringContent(string.Format(
                    "grant_type=password&username={0}&password={1}",
                    username, password),
                    Encoding.UTF8, "application/x-www-form-urlencoded"));
                var resultJSON = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(
                    resultJSON);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Response> CheckConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = Languages.TurnOnInternet,
                };
            }

            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isReachable)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = Languages.NoInternet,
                };
            }

            return new Response
            {
                IsSuccess = true,
            };
        }

        // La clase Response, como es una clase que usaremos transversalmente, no la crearemos aquí, sino que la crearemos en Sales.Common.Models
        public async Task<Response> GetList<T>(string urlBase, string prefix, string controller)
        {
            try
            {
                var cliente = new HttpClient();
                //cliente.BaseAddress = new Uri(urlBase);
                
                var url2 = new Uri(string.Format("{0}{1}{2}", urlBase, prefix, controller)); // ME - tengo que hacer esto por que no me toma 'SalesAPI' en la urlBase nio en el prefix

                var url = $"{prefix}{controller}"; // Esto concatena. Es equivalente al String.format
                var response = await cliente.GetAsync(url2);
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

        public async Task<Response> Post<T>(string urlBase, string prefix, string controller, T model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = $"{prefix}{controller}";
                var response = await client.PostAsync(url, content);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = answer,
                    };
                }

                var obj = JsonConvert.DeserializeObject<T>(answer);
                return new Response
                {
                    IsSuccess = true,
                    Result = obj,
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

        public async Task<Response> Put<T>(string urlBase, string prefix, string controller, T model, int id)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = $"{prefix}{controller}/{id}";
                var response = await client.PutAsync(url, content);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = answer,
                    };
                }

                var obj = JsonConvert.DeserializeObject<T>(answer);
                return new Response
                {
                    IsSuccess = true,
                    Result = obj,
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
        
        public async Task<Response> Delete(string urlBase, string prefix, string controller, int id)
        {
            try
            {
                var cliente = new HttpClient();
                cliente.BaseAddress = new Uri(urlBase);
                var url = $"{prefix}{controller}/{id}"; // Esto concatena. Es equivalente al String.format
                var response = await cliente.DeleteAsync(url);
                var answer = await response.Content.ReadAsStringAsync(); // Aqui tenemos todo el json, pero en formato string. Hay que desserializarlo.
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = answer, // Que muestre lo que leyó en la variable answer
                    };
                }

                return new Response
                {
                    IsSuccess = true,
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
