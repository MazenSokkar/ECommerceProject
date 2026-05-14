export interface Banner {
  id: number;
  title: string;
  content: string | null;
  imageUrl: string;
  linkUrl: string;
  isActive: boolean;
  displayOrder: number;
}

export interface CreateBannerRequest {
  title: string;
  content: string | null;
  imageUrl: string;
  linkUrl: string | null;
  isActive: boolean;
  displayOrder: number;
}

export interface UpdateBannerRequest {
  title: string;
  content: string | null;
  imageUrl: string;
  linkUrl: string | null;
  isActive: boolean;
  displayOrder: number;
}
