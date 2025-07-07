public abstract class TUIPageFooter : TUIControl
{
	public TUIPageFrame frame;

	protected abstract void HandlePageFrameChange(TUIPageFrame pageFrame);

	protected virtual void ConcernFrame(TUIPageFrame frame)
	{
		if (null != this.frame)
		{
			this.frame.RemoveHandler(HandlePageFrameChange);
		}
		this.frame = frame;
		if (null != this.frame)
		{
			this.frame.AddHandler(HandlePageFrameChange);
		}
	}

	protected virtual void Awake()
	{
		ConcernFrame(frame);
	}
}
