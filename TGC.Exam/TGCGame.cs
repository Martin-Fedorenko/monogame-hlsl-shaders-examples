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

        private GraphicsDeviceManager Graphics { get; set; }
        private FreeCamera Camera { get; set; }
        private SpherePrimitive SphereModel { get; set; }
        private FullScreenQuad FullScreenQuad { get; set; }
        private BoxPrimitive Caja { get; set; }
        private Matrix FloorWorld { get; set; }
        private Matrix CajaWorld { get; set; }

        private Texture2D FloorTexture { get; set; }
        private Texture2D Noise { get; set; }
        private Texture2D TexturaAuxiliar { get; set; }
        private Texture2D CajaTexture { get; set; }
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
            Camera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0f, 100f, 50f), screenSize);

            FloorWorld = Matrix.CreateScale(300f) * Matrix.CreateTranslation(0f, 0f, 0f);
            lightPosition = new Vector3(100f, 0f, 0f);

            CajaWorld = Matrix.Identity;
            

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
            FullScreenQuad = new FullScreenQuad(GraphicsDevice);

            // Se carga la textura del piso
            FloorTexture = Content.Load<Texture2D>(ContentFolderTextures + "tiles");

            CajaTexture = Content.Load<Texture2D>(ContentFolderTextures + "cajaa");
            Noise = Content.Load<Texture2D>(ContentFolderTextures + "perlin");
            TexturaAuxiliar = Content.Load<Texture2D>(ContentFolderTextures + "lava");
            

            // Se carga el efecto principal
            
            //--------------------------------------------------CHANGE SHADER HERE
            Effect = Content.Load<Effect>(ContentFolderEffect + "shader (5)");
            //-------------------------------------------------------------------
            
            Effect.Parameters["ambientColor"]?.SetValue(new Vector3(1.0f, 1.0f, 1.0f));
            Effect.Parameters["diffuseColor"]?.SetValue(new Vector3(100.0f, 100.0f, 100.0f));
            Effect.Parameters["specularColor"]?.SetValue(new Vector3(1.0f, 1.0f, 1.0f));

            Effect.Parameters["KAmbient"]?.SetValue(0.1f);
            Effect.Parameters["KDiffuse"]?.SetValue(1.0f);
            Effect.Parameters["KSpecular"]?.SetValue(1.0f);
            Effect.Parameters["shininess"]?.SetValue(100f);

            Vector4 Plano  = new Vector4(-2.0f,-1.0f,0.0f,0.0f);

            Effect.Parameters["plano"]?.SetValue(Plano);

            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);

            var tamCaja = new Vector3(10f,10f,10f);
            Caja = new BoxPrimitive(GraphicsDevice, tamCaja, CajaTexture);

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
            var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Logica de actualizacion
            Camera.Update(gameTime);
            Effect.Parameters["CameraPosition"]?.SetValue(Camera.Position);

            lightPosition = new Vector3(MathF.Cos(Time) * 100f, 45f, MathF.Sin(Time) * 10f);
            //lightPosition = new Vector3(MathF.Sin(Time) * 100f, 45f, MathF.Cos(Time) * 0f);
            Effect.Parameters["lightPosition"]?.SetValue(lightPosition);
            Effect.Parameters["Time"]?.SetValue(Time);

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

            CajaWorld = Matrix.CreateTranslation(0f,50f,0f) * Matrix.CreateScale(2f);
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

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            Effect.Parameters["TexturaRuido"]?.SetValue(Noise);
            Effect.Parameters["TexturaAuxiliar"]?.SetValue(TexturaAuxiliar);
            Effect.Parameters["ModelTexture"]?.SetValue(FloorTexture);
            Effect.CurrentTechnique = Effect.Techniques["BasicShader"];
            Effect.Parameters["World"]?.SetValue(FloorWorld);
            Effect.Parameters["WorldViewProjection"]?.SetValue(FloorWorld * Camera.View * Camera.Projection);
            Effect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Invert(Matrix.Transpose(FloorWorld)));

            Floor.Draw(Effect);
            
            Effect.Parameters["ModelTexture"]?.SetValue(CajaTexture);
            Effect.CurrentTechnique = Effect.Techniques["BasicShader"];
            Effect.Parameters["World"]?.SetValue(CajaWorld);
            Effect.Parameters["WorldViewProjection"]?.SetValue(CajaWorld * Camera.View * Camera.Projection);
            Effect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Invert(Matrix.Transpose(CajaWorld)));

            Caja.Draw(Effect);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            Effect.CurrentTechnique = Effect.Techniques["PostProcessing"];
            Effect.Parameters["ModelTexture"]?.SetValue(RenderTarget);
            FullScreenQuad.Draw(Effect);

            base.Draw(gameTime);
        }

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();
            SphereModel.Dispose();
            FullScreenQuad.Dispose();

            base.UnloadContent();
        }
    }
}