using System;

// Token: 0x02000005 RID: 5
public class ComponentPriority : Attribute
{
	// Token: 0x0600000A RID: 10 RVA: 0x000020FC File Offset: 0x000002FC
	public ComponentPriority(int priority = 0)
	{
		this.Priority = priority;
	}

	// Token: 0x04000003 RID: 3
	public int Priority;
}
