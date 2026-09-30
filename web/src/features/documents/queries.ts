import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import * as documentsApi from '../../api/documents'
import * as foldersApi from '../../api/folders'

const foldersKey = (parentId: string | null) => ['folders', parentId] as const
const folderKey = (id: string) => ['folder', id] as const
const documentsKey = (folderId: string | null) => ['documents', folderId] as const

export function useFolders(parentId: string | null) {
    return useQuery({
        queryKey: foldersKey(parentId),
        queryFn: () => foldersApi.getFolders(parentId),
    })
}

export function useFolder(id: string | null) {
    return useQuery({
        queryKey: id ? folderKey(id) : ['folder', 'root'],
        queryFn: () => foldersApi.getFolder(id as string),
        enabled: id !== null,
    })
}

export function useDocuments(folderId: string | null) {
    return useQuery({
        queryKey: documentsKey(folderId),
        queryFn: () => documentsApi.getDocuments(folderId),
    })
}

export function useCreateFolder(parentId: string | null) {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: (name: string) => foldersApi.createFolder({ name, parentFolderId: parentId }),
        onSuccess: () => {
            void queryClient.invalidateQueries({ queryKey: foldersKey(parentId) })
        },
    })
}

export function useUpdateFolder(parentId: string | null) {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: ({ id, name }: { id: string; name: string }) =>
            foldersApi.updateFolder(id, { name, parentFolderId: parentId }),
        onSuccess: () => {
            void queryClient.invalidateQueries({ queryKey: foldersKey(parentId) })
        },
    })
}

export function useDeleteFolder(parentId: string | null) {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: (id: string) => foldersApi.deleteFolder(id),
        onSuccess: () => {
            void queryClient.invalidateQueries({ queryKey: foldersKey(parentId) })
        },
    })
}

export function useUploadDocument(folderId: string | null) {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: (input: { file: File; name?: string }) =>
            documentsApi.uploadDocument({ ...input, folderId }),
        onSuccess: () => {
            void queryClient.invalidateQueries({ queryKey: documentsKey(folderId) })
        },
    })
}

export function useRetranslateDocument(folderId: string | null) {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: (id: string) => documentsApi.retranslateDocument(id),
        onSuccess: () => {
            void queryClient.invalidateQueries({ queryKey: documentsKey(folderId) })
        },
    })
}

export function useUpdateDocument(folderId: string | null) {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: ({ id, name }: { id: string; name: string }) =>
            documentsApi.updateDocument(id, { name, folderId }),
        onSuccess: () => {
            void queryClient.invalidateQueries({ queryKey: documentsKey(folderId) })
        },
    })
}

export function useDeleteDocument(folderId: string | null) {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: (id: string) => documentsApi.deleteDocument(id),
        onSuccess: () => {
            void queryClient.invalidateQueries({ queryKey: documentsKey(folderId) })
        },
    })
}
