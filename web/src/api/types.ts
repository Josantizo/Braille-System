// Mirrors the DTOs returned by BrailleSuite.Api (Features/Documents/Dtos).
// ASP.NET Core serializes to camelCase by default, hence the casing here.

export interface FolderSummaryDto {
    id: string
    name: string
    parentFolderId: string | null
    createdAt: string
    updatedAt: string
}

export interface BreadcrumbItemDto {
    id: string
    name: string
}

export interface FolderDetailResponse {
    id: string
    name: string
    parentFolderId: string | null
    breadcrumb: BreadcrumbItemDto[]
    createdAt: string
    updatedAt: string
}

export type TranslationStatus = 'Pending' | 'Translated' | 'Failed'

export interface DocumentSummaryDto {
    id: string
    name: string
    folderId: string | null
    originalFileName: string
    originalContentType: string
    translationStatus: TranslationStatus
    translationError: string | null
    hasTranslatedContent: boolean
    createdAt: string
    updatedAt: string
}

export type DocumentDetailResponse = DocumentSummaryDto
