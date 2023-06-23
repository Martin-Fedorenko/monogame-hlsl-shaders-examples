using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGC.Exam
{
    /// <summary>
    ///     Esta es la clase principal del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffect = "Effects/";
        public const string ContentFolderTextures = "Textures/";
        private const bool LightingEnabled = true;

        private GraphicsDeviceManager Graphics { get; set; }
        private FreeCamera Camera { get; set; }
        private SpherePrimitive SphereModel { get; set; }
        //private Quad FloorQuad { get; set; }
        private Model RobotModel { get; set; }
        private FullScreenQuad FullScreenQuad { get; set; }
        private Cube LightCube { get; set; }
        private BoxPrimitive Caja { get; set; }
        //private CubePrimitive Caja { get; set; }
        private Matrix LavaSphereWorld { get; set; }
        private Matrix WaterSphereWorld { get; set; }
        private Matrix RockSphereWorld { get; set; }
        private Matrix RobotWorld { get; set; }
        private Matrix FloorWorld { get; set; }
        private Matrix CajaWorld { get; set; }

        private Texture2D LavaTexture { get; set; }
        private Texture2D WaterTexture { get; set; }
        private Texture2D RockTexture { get; set; }
        private Texture2D FloorTexture { get; set; }
        private Texture2D RobotTexture { get; set; } 
        private Texture2D CajaTexture { get; set; }
        private Vector3 LightOnePosition { get; set; }
        private Vector3 LightTwoPosition { get; set; }
        private Effect Effect { get; set; }
        private RenderTarget2D RenderTarget { get; set; }
        private QuadPrimitive Floor { get; set; }

        private Model FloorModel { get; set; }

        private Vector3 lightPosition;

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            // Descomentar para que el juego sea pantalla completa.
            // Graphics.IsFullScreen = true;
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";
            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            Graphics.ApplyChanges();

            var screenSize = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Camera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0f, 5f, 20f), screenSize);

            RobotWorld = Matrix.CreateScale(0.35f) * Matrix.CreateTranslation(0f, 45f, 0f);
            LavaSphereWorld = Matrix.Identity;
            WaterSphereWorld = Matrix.CreateTranslation(-40f, 0f, 0f);
            RockSphereWorld = Matrix.CreateTranslation(40f, 0f, 0f);
            FloorWorld = Matrix.CreateScale(300f) * Matrix.CreateTranslation(0f, -10f, 0f);
            lightPosition = new Vector3(100f, 45f, 0f);

            CajaWorld = Matrix.CreateScale(2f) * Matrix.CreateTranslation(50f, 45f, 0f);

            

            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el
        ///     procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            // Se cargan los modelos
            SphereModel = new SpherePrimitive(GraphicsDevice, 15f, 8);
            Floor = new QuadPrimitive(GraphicsDevice);
            //FloorQuad = new Quad(GraphicsDevice);
            //FloorModel = Content.Load<Model>(ContentFolder3D + "plano");
            RobotModel = Content.Load<Model>(ContentFolder3D + "tgcito-classic/tgcito-classic");
            FullScreenQuad = new FullScreenQuad(GraphicsDevice);

            // Se cargan las texturas de las esferas
            LavaTexture = Content.Load<Texture2D>(ContentFolderTextures + "lava");
            WaterTexture = Content.Load<Texture2D>(ContentFolderTextures + "water");
            RockTexture = Content.Load<Texture2D>(ContentFolderTextures + "stones");

            // Se carga la textura del piso
            FloorTexture = Content.Load<Texture2D>(ContentFolderTextures + "tiles");

            // Se carga la textura del robot
            var robotEffect = RobotModel.Meshes.FirstOrDefault().Effects.FirstOrDefault();
            RobotTexture = robotEffect.Parameters["Texture"].GetValueTexture2D();

            CajaTexture = Content.Load<Texture2D>(ContentFolderTextures + "cajaa");

            // Se carga el efecto principal
            Effect = Content.Load<Effect>(ContentFolderEffect + "TgcExamShader4");

            Effect.Parameters["ambientColor"]?.SetValue(new Vector3(1.0f, 1.0f, 1.0f));
            Effect.Parameters["diffuseColor"]?.SetValue(new Vector3(100.0f, 100.0f, 100.0f));
            Effect.Parameters["specularColor"]?.SetValue(new Vector3(1.0f, 1.0f, 1.0f));

            Effect.Parameters["KAmbient"]?.SetValue(0.1f);
            Effect.Parameters["KDiffuse"]?.SetValue(1.0f);
            Effect.Parameters["KSpecular"]?.SetValue(1.0f);
            Effect.Parameters["shininess"]?.SetValue(100f);

            

            foreach (var mesh in RobotModel.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    part.Effect = Effect;
                }
            }

            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);

            var tamCaja = new Vector3(10f,10f,10f);
            Caja = new BoxPrimitive(GraphicsDevice, tamCaja, CajaTexture);
            

            if (LightingEnabled)
            {
                LightCube = new Cube(GraphicsDevice);

                Effect.Parameters["LightOneColor"]?.SetValue(Color.Red.ToVector3());
                Effect.Parameters["LightTwoColor"]?.SetValue(Color.Blue.ToVector3());
            }

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            var Time = (float)gameTime.TotalGameTime.TotalSeconds;

            
            // Logica de actualizacion
            Camera.Update(gameTime);
            Effect.Parameters["CameraPosition"]?.SetValue(Camera.Position);

            if (LightingEnabled)
            {
                
                LightOnePosition = new Vector3(MathF.Cos(Time) * 20f, 0f, MathF.Sin(Time) * 20f);
                LightTwoPosition = new Vector3(-MathF.Sin(Time) * 20f, 0f, MathF.Cos(Time) * 20f);
                Effect.Parameters["LightOnePosition"]?.SetValue(LightOnePosition);
                Effect.Parameters["LightTwoPosition"]?.SetValue(LightTwoPosition);
                Effect.Parameters["Time"]?.SetValue(Time);
            }
            lightPosition = new Vector3(MathF.Cos(Time) * 100f, 45f, MathF.Sin(Time) * 0f);
            lightPosition = new Vector3(MathF.Sin(Time) * 100f, 45f, MathF.Cos(Time) * 0f);
            Effect.Parameters["lightPosition"]?.SetValue(lightPosition);

            //LavaSphereWorld
            
            // Capturar Input teclado
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                //Salgo del juego.
                Exit();
            }

            var escalado = new Matrix( //clave la matriz custom
            1f, 0f, 0f, 0f,
            0f, 0.5f*(float)Math.Sin(5f*Time) + 0.5f, 0f, 0f,
            0f, 0f, 1f, 0f,
            0f, 0f, 0f, 1f
            );


            //LavaSphereWorld =  escalado * Matrix.CreateTranslation(posicion);
            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el código referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(Color.Black);

            SphereModel.Draw(LavaSphereWorld, Camera.View, Camera.Projection, LavaTexture);
            SphereModel.Draw(WaterSphereWorld, Camera.View, Camera.Projection, WaterTexture);
            SphereModel.Draw(RockSphereWorld, Camera.View, Camera.Projection, RockTexture);

            //FloorQuad.Effecto = Effect;
            //FloorQuad.Draw(FloorWorld, Camera.View, Camera.Projection, FloorTexture);

            Effect.Parameters["ModelTexture"]?.SetValue(FloorTexture);
            Effect.CurrentTechnique = Effect.Techniques["BasicShader"];
            Effect.Parameters["World"]?.SetValue(FloorWorld);
            Effect.Parameters["WorldViewProjection"]?.SetValue(FloorWorld * Camera.View * Camera.Projection);
            Effect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Invert(Matrix.Transpose(FloorWorld)));

            Floor.Draw(Effect);
            
            DrawRobot();
            
            Effect.Parameters["ModelTexture"]?.SetValue(CajaTexture);
            Effect.CurrentTechnique = Effect.Techniques["BasicShader"];
            Effect.Parameters["World"]?.SetValue(CajaWorld);
            Effect.Parameters["WorldViewProjection"]?.SetValue(CajaWorld * Camera.View * Camera.Projection);
            Effect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Invert(Matrix.Transpose(CajaWorld)));

            Caja.Draw(Effect);

            if(LightingEnabled)
            {
                LightCube.Draw(Matrix.CreateTranslation(LightOnePosition), Camera.View, Camera.Projection, Color.Red);
                LightCube.Draw(Matrix.CreateTranslation(LightTwoPosition), Camera.View, Camera.Projection, Color.Blue);
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            Effect.CurrentTechnique = Effect.Techniques["PostProcessing"];
            Effect.Parameters["ModelTexture"].SetValue(RenderTarget);
            FullScreenQuad.Draw(Effect);

            base.Draw(gameTime);
        }

        private void DrawRobot()
        {
            Effect.Parameters["ModelTexture"].SetValue(RobotTexture);
            Effect.CurrentTechnique = Effect.Techniques["BasicShader"];

            foreach (var mesh in RobotModel.Meshes)
            {
                var world = mesh.ParentBone.Transform * RobotWorld;
                Effect.Parameters["World"]?.SetValue(world);
                Effect.Parameters["WorldViewProjection"]?.SetValue(world * Camera.View * Camera.Projection);
                Effect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Invert(Matrix.Transpose(world)));
                mesh.Draw();
            }

        }

        private void DrawFloor()
        {
            Effect.Parameters["ModelTexture"].SetValue(FloorTexture);
            Effect.CurrentTechnique = Effect.Techniques["BasicShader"];

            foreach (var mesh in FloorModel.Meshes)
            {
                var world = mesh.ParentBone.Transform * FloorWorld;
                Effect.Parameters["World"]?.SetValue(world);
                Effect.Parameters["WorldViewProjection"]?.SetValue(FloorWorld * Camera.View * Camera.Projection);
                Effect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Invert(Matrix.Transpose(FloorWorld)));
                mesh.Draw();
            }

        }

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            //FloorQuad.Dispose();
            SphereModel.Dispose();
            FullScreenQuad.Dispose();

            if (LightingEnabled)
            {
                LightCube.Dispose();
            }

            base.UnloadContent();
        }
    }
}