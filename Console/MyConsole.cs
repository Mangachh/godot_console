using System;

// usamos el namespace de godot y así sólo tenemos que meter Console.Write -> etc;
namespace Godot
{

    public static class MyConsole
    {   
        /// <summary>
        /// is the console on?
        /// </summary>
        private static bool isOn;

        /// <summary>
        /// Path3D to the <see cref="ConsoleUI">.
        /// <p>Here is where the visual is.
        /// </summary>
        private const string CONSOLEPATH = "res://Console/Console.tscn";

        /// <summary>
        /// RefCounted to the <see cref="ConsoleUI">
        /// </summary>
        public static ConsoleUI Console{private get; set;}

        public static bool IsOn
        {
            get => isOn;
            set
            {
                isOn = value;

                if (isOn)
                {
                    //OpenConsole();
                    
                }
                else
                {
                    //CloseConsole();
                }
            }
        }

        /// <summary>
        /// Writes a message on the debugger and on the <see cref="ConsoleUI"> 
        /// if <see cref="MyConsole.IsOn"> is true.
        /// The color is setted in <see cref="ConsoleUI.COLORDEFAULT">
        /// </summary>
        /// <param name="message">the message to be written</param>
        public static void Write(in string message)
        {
            GD.Print(message);

            if (isOn)
            {
                Console.WriteMessage(message);
            }
        }

        public static void Write(in object obj){
            GD.Print(obj);

            if(isOn){
                Console.WriteMessage(obj.ToString());
            }
        }

        /// <summary>
        /// Writes a message on the debugger and on the <see cref="ConsoleUI"> 
        /// if <see cref="MyConsole.IsOn"> is true.
        /// Here the call uses a custom color
        /// </summary>
        /// <param name="message">the message to be written</param>
        public static void Write(in string message, in Color color)
        {
            GD.Print(message);

            if (isOn)
            {
                Console.WriteOnConsole(message, color);
            }
        }

        public static void Write(in object obj, in Color color)
        {
            GD.Print(obj);

            if (isOn)
            {
                Console.WriteOnConsole(obj.ToString(), color);
            }
        }

        /// <summary>
        /// Writes a Warning message on the debugger and on the <see cref="ConsoleUI"> 
        /// if <see cref="MyConsole.IsOn"> is true.
        /// The color is setted in <see cref="ConsoleUI.COLORWARNING">
        /// </summary>
        /// <param name="message">the message to be written</param>
        public static void WriteWarning(in string message)
        {
            GD.PushWarning(message);

            if (isOn)
            {
                Console.WriteWarning(message);
            }
        }

        /// <summary>
        /// Writes an Error message on the debugger and on the <see cref="ConsoleUI"> 
        /// if <see cref="MyConsole.IsOn"> is true.
        /// The color is setted in <see cref="ConsoleUI.COLORERROR">
        /// </summary>
        /// <param name="message">the message to be written</param>
        public static void WriteError(in string message)
        {
            GD.PrintErr(message);

            if (isOn)
            {
                Console.WriteError(message);
            }
        }

        /// <summary>
        /// Opens the console loading the <see cref="ConsoleUI"/> and setting it 
        /// to the <see cref="AppManagerGo"> as a children node.
        /// </summary>
        [Obsolete ("Doesn't work.")]
        private static void OpenConsole()
        {
            PackedScene sc = GD.Load<PackedScene>(CONSOLEPATH);
            Console = sc.Instantiate<ConsoleUI>();
            GD.Print("Console no UI created");
            GD.Print(Console);
            //MySystems.SystemManager.GetInstance(console).NodeManager.CallDeferred("addchild", console);
            
            Console.Init();
        }

        /// <summary>
        /// We dispose the console.
        /// </summary>
        [Obsolete ("Doesn't work.")]
        private static void CloseConsole()
        {
            try
            {
                Console.Dispose();
                Console = null;
            }
            catch (Exception e)
            {
                GD.PrintErr(e.Message);
            }

        }

       
    }
}
