using Coconut.Core.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileProxy
{
    public Profile Data { get; private set; }
    private Queue<Sprite> _photoQueue;

    //---------------------------------------------------

    public ProfileProxy(Profile profile)
    {
        Data = profile;

        GeneratePhotoQueue();
    }

    public Sprite GetPhotoFromQueue()
    {
        if (_photoQueue?.Count == 0)
        {
            GeneratePhotoQueue();
        }
        
        var photo = _photoQueue.Dequeue();
        return photo;
    }

    //---------------------------------------------------

    private void GeneratePhotoQueue()
    {
        var photos = new Sprite[Data.photos.Length];
        Data.photos.CopyTo(photos, 0);
        photos.Shuffle();

        _photoQueue = new Queue<Sprite>(photos);
    }
}