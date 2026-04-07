using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Feedback;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;
        public FeedbackService(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }
        public async Task<List<FeedbackDTO>> GetFeedbacksByBookingItineraryId(Guid bookingItineraryId)
        {
            var list = await _feedbackRepository.GetFeedbacksByBookingItineraryIdAsync(bookingItineraryId);
            return _mapper.Map<List<FeedbackDTO>>(list);
        }
        public async Task<FeedbackDTO> GetFeedback(Guid id)
        {
            var feedback = await _feedbackRepository.GetFeedback(id);
            return _mapper.Map<FeedbackDTO>(feedback);
        }
        public async Task<List<FeedbackDTO>> GetFeedbacks(FeedbackSearch search)
        {
            var list = await _feedbackRepository.GetFeedbacks(search);
            return _mapper.Map<List<FeedbackDTO>>(list);
        }
        public async Task<List<FeedbackDTO>> GetFeedbacks()
        {
            var list = await _feedbackRepository.GetFeedbacks();
            return _mapper.Map<List<FeedbackDTO>>(list);
        }
        public async Task<FeedbackDTO> AddFeedback(FeedbackCreateRequest create)
        {
            var feedback = _mapper.Map<Feedback>(create);
            var result = await _feedbackRepository.CreateAsync(feedback);
            return _mapper.Map<FeedbackDTO>(result);
        }
        public async Task<bool> DeleteFeedback(Guid id)
        {
            var result = await _feedbackRepository.DeleteAsync(id);
            return result;
        }
        public async Task<FeedbackDTO> UpdateFeedback(Guid id, FeedbackUpdateRequest update)
        {
            var feedback = _mapper.Map<Feedback>(update);
            feedback.Id = id;
            var result = await _feedbackRepository.UpdateAsync(feedback);
            return _mapper.Map<FeedbackDTO>(result);
        }
    }
    }
