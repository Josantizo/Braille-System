import { apiDelete, apiGet, apiPost, apiPut } from './client'
import type { FolderDetailResponse, FolderSummaryDto } from './types'

export function getFolders(parentId: string | null): Promise<FolderSummaryDto[]> {
    const query = parentId ? `?parentId=${parentId}` : ''
    return apiGet<FolderSummaryDto[]>(`/api/folders${query}`)
}

export function getFolder(id: string): Promise<FolderDetailResponse> {
    return apiGet<FolderDetailResponse>(`/api/folders/${id}`)
}

export function createFolder(input: { name: string; parentFolderId: string | null }): Promise<FolderDetailResponse> {
    return apiPost<FolderDetailResponse>('/api/folders', input)
}

export function updateFolder(
    id: string,
    input: { name: string; parentFolderId: string | null },
): Promise<FolderDetailResponse> {
    return apiPut<FolderDetailResponse>(`/api/folders/${id}`, input)
}

export function deleteFolder(id: string): Promise<void> {
    return apiDelete(`/api/folders/${id}`)
}
