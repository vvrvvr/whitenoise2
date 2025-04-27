using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Datamosh : ScriptableRendererFeature
{
	[SerializeField] private Material _blitMaterial;
	[SerializeField] private RenderTexture _camRenderTexture;
	[SerializeField] private int _fps = 30;
	
	private DatamoshPass _renderPass;

	public bool Enabled { get=> _renderPass.Enabled; set=> _renderPass.Enabled = value; }

	/*
	Called when the Renderer Feature loads the first time,
	when you enable or disable the Renderer Feature, or
	when you change a property in the inspector of the Renderer Feature.
	*/
	public override void Create()
	{
		if (_blitMaterial == null || _camRenderTexture == null) return;
		_renderPass = new DatamoshPass(_blitMaterial, _camRenderTexture, _fps);
		_renderPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
	}

	/*
	Unity calls this method every frame, once for each camera. This method lets
	you inject ScriptableRenderPass instances into the scriptable Renderer.
	*/
	public override void AddRenderPasses(ScriptableRenderer renderer,
		ref RenderingData renderingData)
	{
		if (_blitMaterial == null || _camRenderTexture == null) return;
		if (renderingData.cameraData.cameraType == CameraType.Game)
		{
			_renderPass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Motion);
			renderer.EnqueuePass(_renderPass);
		}
	}

    protected override void Dispose(bool disposing)
    {
        _renderPass.Dispose();
    }
}

public class DatamoshPass : ScriptableRenderPass
{
	private Material _material;
	private RenderTexture _rt;
	private RTHandle _rtHandle;
	private int _fps;

	public bool Enabled { get; set; }

	public DatamoshPass(Material material, RenderTexture rt, int fps)
	{
		_material = material;
		_rt = rt;
		_fps = fps;
		
		// _rtTextureDescriptor = new RenderTextureDescriptor(Screen.width,
		// 	Screen.height, RenderTextureFormat.Default, 0);

		//_rt = new RenderTexture(_rtTextureDescriptor);
		
		_rtHandle = RTHandles.Alloc(_rt);
	}

	// Called before execute
	public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
	{
		// _blurTextureDescriptor.width = cameraTextureDescriptor.width;
   		// _blurTextureDescriptor.height = cameraTextureDescriptor.height;

		// RenderingUtils.ReAllocateIfNeeded(ref _blurTextureHandle, _blurTextureDescriptor);
	}

	// Called every frame
	private float _lastFrame;
	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		if (!Enabled) return;

		CommandBuffer cmd = CommandBufferPool.Get();
		RTHandle cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

		if (Time.timeAsDouble > _lastFrame + 1.0/_fps)
		{
			Blit(cmd, _rtHandle, cameraTargetHandle, _material);
			_lastFrame = Time.time;
		}
		else
		{
			Blit(cmd, _rtHandle, cameraTargetHandle);
		}
		
		context.ExecuteCommandBuffer(cmd);
		CommandBufferPool.Release(cmd);
	}

	public void Dispose()
	{
		//_rtHandle?.Release();
	}
}