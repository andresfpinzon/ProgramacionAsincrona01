using System;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;
using Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace AsyncBreakfast
{
    // These classes are intentionally empty for the purpose of this example. They are
    //simply marker classes for the purpose of demonstration, contain no properties, and
    //serve no other purpose.
 internal class Bacon { }
    //internal class Coffee { }
    //internal class Egg { }
    //internal class Juice { }
    //internal class Toast { }
    class Program
    {
        //En esta seccion cada tarea empezara cuando la anterior termine, por lo tanto en un entorno real, si empezamos realizando el cafe
        //y terminamos tostando el pan 30 minutos despues, cuando sirvamos el desayuno, el cafe ya estaria frio, por tanto no es la manera mas eficiente de preparar el desayuno.

        //static void Main(string[] args)
        //{
        //    Coffee cup = PourCoffee();
        //    Console.WriteLine("coffee is ready");
        //    Egg eggs = FryEggs(2);
        //    Console.WriteLine("eggs are ready");
        //    Bacon bacon = FryBacon(3);
        //    Console.WriteLine("bacon is ready");
        //    Toast toast = ToastBread(2);
        //    ApplyButter(toast);
        //    ApplyJam(toast);
        //    Console.WriteLine("toast is ready");
        //    Juice oj = PourOJ();
        //    Console.WriteLine("oj is ready");
        //    Console.WriteLine("Breakfast is ready!");
        //}

        //Comencemos por actualizar este código para que el hilo no se bloquee mientras se ejecutan las tareas.
        //La palabra clave await proporciona una forma no bloqueante de iniciar una tarea y luego continuar la ejecución cuando esa tarea se completa
        

        static async Task Main(string[] args)
        {
            Coffee cup = PourCoffee();
            Console.WriteLine("coffee is ready");
            var eggsTask = FryEggsAsync(2);
            var baconTask = FryBaconAsync(3);
            var toastTask = MakeToastWithButterAndJamAsync(2);
            var breakfastTasks = new List<Task> { eggsTask, baconTask, toastTask
};          // Para esperar tareas de forma eficiente una  opción es usar WhenAny, que devuelve un Task<Task> que se completa
            // cuando cualquiera de sus argumentos se completa. Puede esperar la tarea devuelta, sabiendo que ya ha finalizado.
            while (breakfastTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(breakfastTasks);
                if (finishedTask == eggsTask)
                {
                    Console.WriteLine("eggs are ready");
                }
                else if (finishedTask == baconTask)
                {
                    Console.WriteLine("bacon is ready");
                }
                else if (finishedTask == toastTask)
                {
                    Console.WriteLine("toast is ready");
                }
                await finishedTask;
                breakfastTasks.Remove(finishedTask);
                //Al final podemos ver  la línea await finishedTask;. La línea await Task.WhenAny no
                //espera la tarea terminada. Espera la Task devuelta por Task.WhenAny.El resultado de
                //Task.WhenAny es la tarea que se ha completado(o con error). Debe volver a usar await
                //con esa tarea, aunque sepa que ya ha finalizado.Así es como recupera su resultado o
                //se asegura de que se lance la excepción que la causó el error.
            }
            Juice oj = PourOJ();
            Console.WriteLine("oj is ready");
            Console.WriteLine("Breakfast is ready!");
        }

        //El siguiente paso es crear métodos que representen la combinación de otro trabajo.Antes de servir el desayuno,
        //debe esperar la tarea que representa el tostado del pan antes de agregar mantequilla y mermelada.
        //El método siguiente tiene el modificador async en su firma. Esto le indica al compilador que este método contiene una instrucción await; contiene operaciones asíncronas.
        static async Task<Toast> MakeToastWithButterAndJamAsync(int number)
        {
            var toast = await ToastBreadAsync(number);
            ApplyButter(toast);
            ApplyJam(toast);
            return toast;
        }
        private static Juice PourOJ()
        {
            Console.WriteLine("Pouring orange juice");
            return new Juice();
        }
        private static void ApplyJam(Toast toast) =>
        Console.WriteLine("Putting jam on the toast");
        private static void ApplyButter(Toast toast) =>
        Console.WriteLine("Putting butter on the toast");

        //Las tareas lanzan excepciones cuando no se pueden completar con éxito.El código del cliente puede capturar esas excepciones  
        //cuando se espera una tarea iniciada.Por ejemplo, supongamos que la tostadora se incendia mientras se prepara la tostada.
        //private static async Task<Toast> ToastBreadAsync(int slices)

        //{
        //    for (int slice = 0; slice < slices; slice++)
        //    {
        //        Console.WriteLine("Putting a slice of bread in the toaster");
        //    }
        //    Console.WriteLine("Start toasting...");
        //    await Task.Delay(2000);
        //    Console.WriteLine("Fire! Toast is ruined!");
        //    throw new InvalidOperationException("The toaster is on fire");
        //    await Task.Delay(1000);
        //    Console.WriteLine("Remove toast from toaster");
        //    return new Toast();
        //}

        //Observará que se completan bastantes tareas entre el momento en que la tostadora se incendia y el momento en que se observa 
        //la excepción.Cuando una tarea que se ejecuta de forma asíncrona lanza una excepción, esa tarea se convierte en una tarea con 
        //error (faulted). El objeto Task almacena la excepción lanzada en la propiedad Task.Exception.
        //Las tareas con error lanzan una excepción cuando se usa await con ellas.

        // la sigueibnte es la funcion anterior sin manejar la advertencia.
        private static async Task<Toast> ToastBreadAsync(int slices)
        {
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("Putting a slice of bread in the toaster");
            }
            Console.WriteLine("Start toasting...");
            await Task.Delay(3000);
            Console.WriteLine("Remove toast from toaster");
            return new Toast();
        }
        private static async Task<Bacon> FryBaconAsync(int slices)
        {
            Console.WriteLine($"putting {slices} slices of bacon in the pan");
            Console.WriteLine("cooking first side of bacon...");
            await Task.Delay(3000);
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("flipping a slice of bacon");
            }
            Console.WriteLine("cooking the second side of bacon...");
            await Task.Delay(3000);
            Console.WriteLine("Put bacon on plate");
            return new Bacon();
        }
        private static async Task<Egg> FryEggsAsync(int howMany)
        {
            Console.WriteLine("Warming the egg pan...");
            await Task.Delay(3000);
            Console.WriteLine($"cracking {howMany} eggs");
            Console.WriteLine("cooking the eggs ...");
            await Task.Delay(3000);
            Console.WriteLine("Put eggs on plate");
            return new Egg();
        }
        private static Coffee PourCoffee()
        {
            Console.WriteLine("Pouring coffee");
            return new Coffee();
        }
    }
}

//La versión final del desayuno preparado de forma asíncrona tardó aproximadamente 6
//minutos porque algunas tareas se ejecutaron simultáneamente. Además, el código
//monitoreó varias tareas a la vez y solo tomó acción cuando fue necesario.
//Si lo comparamos con la version inicial del codigo donde la preparacion del desayuno se
//demoraba unos 30 minutos, podemos ver como usar tareas asincronas beneficia la ejecucion del programa.

