namespace LupiraCareerApi.Domain;

public enum MediaRole
{
    Hero,
    Gallery,
    Thumbnail,
}

public enum MediaTargetKind
{
    Project,
    Skill,
}

/// <summary>An image/media asset (blob stored on the shared MinIO; <see cref="BlobRef"/> is the object key)
/// illustrating projects and skills. Event-sourced; one stream per asset, owned by a single
/// <see cref="OwnerPrincipalId"/>.</summary>
public class MediaAsset
{
    public Guid Id { get; set; }
    public int Version { get; set; }

    public Guid OwnerPrincipalId { get; set; }

    public string BlobRef { get; set; } = "";
    public string MimeType { get; set; } = "";
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string AltText { get; set; } = "";
    public string? Caption { get; set; }
    public bool Archived { get; set; }

    public List<ProjectLink> LinkedProjects { get; set; } = new();
    public List<Guid> LinkedSkillIds { get; set; } = new();

    public void Apply(MediaRegistered e)
    {
        Id = e.MediaId;
        OwnerPrincipalId = e.OwnerPrincipalId;
        BlobRef = e.BlobRef;
        MimeType = e.MimeType;
        Width = e.Width;
        Height = e.Height;
        AltText = e.AltText;
        Caption = e.Caption;
    }

    public void Apply(MediaLinkedToProject e)
    {
        if (LinkedProjects.All(p => p.ProjectId != e.ProjectId))
            LinkedProjects.Add(new ProjectLink { ProjectId = e.ProjectId, Role = e.Role });
    }

    public void Apply(MediaLinkedToSkill e)
    {
        if (!LinkedSkillIds.Contains(e.SkillId))
            LinkedSkillIds.Add(e.SkillId);
    }

    public void Apply(MediaUnlinked e)
    {
        switch (e.TargetKind)
        {
            case MediaTargetKind.Project:
                LinkedProjects.RemoveAll(p => p.ProjectId == e.TargetId);
                break;
            case MediaTargetKind.Skill:
                LinkedSkillIds.Remove(e.TargetId);
                break;
        }
    }

    public void Apply(MediaReplaced e)
    {
        BlobRef = e.NewBlobRef;
        MimeType = e.NewMimeType;
        Width = e.NewWidth;
        Height = e.NewHeight;
    }

    public void Apply(MediaArchived e) => Archived = true;
}

public sealed class ProjectLink
{
    public Guid ProjectId { get; set; }
    public MediaRole Role { get; set; }
}
